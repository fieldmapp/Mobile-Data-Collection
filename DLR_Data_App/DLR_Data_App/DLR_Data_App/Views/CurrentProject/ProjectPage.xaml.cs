using System;
using System.Collections.Generic;
using System.Linq;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using DLR_Data_App.ViewModels.CurrentProject;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProjectPage
	{
    public List<string> elementNameList = new List<string>();
    public List<string> elementValueList = new List<string>();

    private readonly ProjectViewModel _viewModel = new ProjectViewModel();
    private Project _workingProject = Database.GetCurrentProject();
    private List<ContentPage> _pages;

    public ProjectPage ()
		{
			InitializeComponent ();

      WalkElements(ref elementNameList, ref elementValueList, false);
      BindingContext = _viewModel;
		}

    /**
     * Generating forms from parsed information
     * @param form Elements that should be generated
     */
    private ContentPage GenerateForm(ProjectForm form)
    {
      var contentPage = new ContentPage();
      var scrollView = new ScrollView();
      var stack = new StackLayout();

      contentPage.Padding = new Thickness(10, 10, 10, 10);

      // walk through list of elements and generate form containing elements
      foreach (var element in form.ElementList)
      {
        contentPage.Title = form.Title;

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        
        // show name of element
        var label = new Label
        {
          Text = Parser.LanguageJson(element.Label, _workingProject.Languages)
        };
        //stack.Children.Add(label);
        grid.Children.Add(label, 0, 0);

        //------------------------
        // Special commands

        var helpButton = new Button()
        {
          Text = AppResources.help
        };
        helpButton.Clicked += async (sender, args) => await DisplayAlert(AppResources.help, Parser.LanguageJson(element.Hint, _workingProject.Languages),
          AppResources.okay);
        //stack.Children.Add(helpButton);
        grid.Children.Add(helpButton, 1, 0);

        // Display a ruler on the side of the screen
        if (element.Type == "inputText" && element.Name.Contains("propRuler"))
        {
          continue;
        }

        // Display a checkbox with name "unknown"
        if (element.Type == "inputText" && element.Name.Contains("unknown"))
        {
          continue;
        }

        //-------------------------
        // Normal fields
        switch (element.Type)
        {
          // input text
          case "inputText":
            {
              var entry = new Entry
              {
                Placeholder = Parser.LanguageJson(element.Hint, _workingProject.Languages),
                Keyboard = Keyboard.Default,
                StyleId = element.Name
              };

              grid.Children.Add(entry, 0, 1);
              Grid.SetColumnSpan(entry, 2);
              break;
            }

          // Selecting one item of list
          case "inputSelectOne":
          {
            var elementList = new List<string>();
              var options = Parser.ParseOptionsFromJson(element.Options);
              foreach (var option in options)
              {
                option.text.TryGetValue("0", out var value);
                elementList.Add(value);
              }
              var picker = new Picker
              {
                Title = Parser.LanguageJson(element.Label, _workingProject.Languages),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                StyleId = element.Name,
                ItemsSource = elementList
              };

              grid.Children.Add(picker, 0, 1);
              Grid.SetColumnSpan(picker, 2);
              break;
            }

          // Show an entry with only numeric input
          // As a walk around for an existing Samsung keyboard bug a normal keyboard layout is used
          case "inputNumeric":
            {
              var entry = new Entry
              {
                Placeholder = Parser.LanguageJson(element.Hint, _workingProject.Languages),
                Keyboard = Keyboard.Default,
                StyleId = element.Name
              };

              grid.Children.Add(entry, 0, 1);
              Grid.SetColumnSpan(entry, 2);
              break;
            }

          // Show current position
          case "inputLocation":
          {
            //var gpsTask = Sensor.GetGps();
            //var gps = gpsTask.Result;

            var labelLat = new Label()
            {
              Text = "Latitude"
            };

            var labelLatData = new Label()
            {
              //Text = gps.Latitude.ToString(CultureInfo.CurrentCulture),
              Text = "",
              StyleId = element.Name + "Lat"
            };

            var labelLong = new Label()
            {
              Text = "Longitude"
            };

            var labelLongData = new Label()
            {
              //Text = gps.Longitude.ToString(CultureInfo.CurrentCulture),
              Text = "",
              StyleId = element.Name + "Long"
            };

            grid.Children.Add(labelLat, 0, 1);
            grid.Children.Add(labelLatData, 1, 1);
            grid.Children.Add(labelLong, 0, 2);
            grid.Children.Add(labelLongData, 1, 2);
            break;
          }
        }
        stack.Children.Add(grid);
      }

      scrollView.Content = stack;
      contentPage.Content = scrollView;
      return contentPage;
    }

    /**
     * Refresh view
     */
    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (Children.Count != 0) return;

      foreach (var page in UpdateView())
      {
        Children.Add(page);
      }
    }

    /**
     * Update view
     */
    public List<ContentPage> UpdateView()
    {
      // Get current project
      _workingProject = Database.GetCurrentProject();
      _pages = new List<ContentPage>();

      // Check if current project is set
      if (_workingProject == null)
      {
        Title = AppResources.currentproject;
      }
      else
      {
        var translatedProject = Helpers.TranslateProjectDetails(_workingProject);
        Title = translatedProject.Title;

        foreach (var projectForm in _workingProject.FormList)
        {
          _pages.Add(GenerateForm(projectForm));
        }
      }

      return _pages;
    }

    /**
     * Navigate to edit page
     */
    private void EditClicked(object sender, EventArgs e)
    {
      Navigation.PushAsync(new EditDataPage(elementNameList));
    }

    /**
     * Save data
     */
    private async void SaveClicked(object sender, EventArgs e)
    {
      WalkElements(ref elementNameList, ref elementValueList, false);

      // Bugfix Tablename empty
      var tableName = Parser.LanguageJsonStandard(_workingProject.Title, _workingProject.Languages) + "_" + _workingProject.Id;
      var status = Database.InsertCustomValues(tableName, elementNameList, elementValueList);

      string message;
      if (status)
      {
        message = AppResources.successful;
        WalkElements(ref elementNameList, ref elementValueList, true);
      }
      else
      {
        message = AppResources.failed;
      }

      await DisplayAlert(AppResources.save, message, AppResources.okay);
    }

    /**
     * Walk through all elements
     *
     * @param elementNameList reference to list of element names
     * @param elementValueList reference to list of element values
     * @param bool if true resets content of element
     */
    private void WalkElements(ref List<string> elementNameList, ref List<string> elementValueList, bool clean)
    {
      // walk trough all pages
      foreach (var page in _pages)
      {
        // walk trough all elements in content page
        // https://forums.xamarin.com/discussion/21032/how-to-get-the-children-of-a-contentpage-or-in-a-view
        foreach (var child in page.Content.LogicalChildren.Where(x => true))
        {
          // https://forums.xamarin.com/discussion/72424/how-to-add-entry-control-dynamically-and-retrieve-its-value
          if (!(child is StackLayout stack)) continue;

          foreach (var view in stack.Children)
          {
            if (!(view is Grid grid)) continue;

            foreach (var element in grid.Children)
            {
              // Store information from element in list, access element by different type
              if (element is Entry entry)
              {
                if (clean)
                {
                  entry.Text = "";
                }
                else
                {
                  elementNameList.Add(entry.StyleId);
                  // check if entry is NULL, if true set default value
                  elementValueList.Add(entry.Text ?? "0");
                }
              }

              if (element is Picker picker)
              {
                if (clean)
                {
                  picker.SelectedIndex = 0;
                }
                elementNameList.Add(picker.StyleId);
                // check if entry is NULL, if true set default value
                elementValueList.Add(picker.SelectedItem as string ?? "0");
              }
            }
          }
        }
      }
    }
  }
}