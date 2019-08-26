using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.CurrentProject
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditDataDetailPage
	{
    private Project _workingProject = Database.GetCurrentProject();

    public EditDataDetailPage ()
		{
			InitializeComponent ();
      foreach (var form in _workingProject.FormList)
      {
        GenerateForm(form);
      }
      
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

      // walk through list of elements and generate form containing elements
      foreach (var element in form.ElementList)
      {
        contentPage.Title = Parser.LanguageJson(element.Label, _workingProject.Languages);

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

              //stack.Children.Add(entry);
              grid.Children.Add(entry, 0, 1);
              Grid.SetColumnSpan(entry, 2);
              break;
            }

          // Selecting one item of list
          case "inputSelectOne":
            {
              var picker = new Picker
              {
                Title = Parser.LanguageJson(element.Label, _workingProject.Languages),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                StyleId = element.Name
              };

              //stack.Children.Add(picker);
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

              //stack.Children.Add(entry);
              grid.Children.Add(entry, 0, 1);
              Grid.SetColumnSpan(entry, 2);
              break;
            }
        }
        stack.Children.Add(grid);
      }

      scrollView.Content = stack;
      contentPage.Content = scrollView;
      return contentPage;
    }
  }
}