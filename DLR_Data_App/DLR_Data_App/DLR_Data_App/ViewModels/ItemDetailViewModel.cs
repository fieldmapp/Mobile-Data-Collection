using System;

using DLR_Data_App.Models;

namespace DLR_Data_App.ViewModels
{
  public class ItemDetailViewModel : BaseViewModel
  {
    public Item Item { get; set; }
    public ItemDetailViewModel(Item item = null)
    {
      Title = item?.Text;
      Item = item;
    }
  }
}
