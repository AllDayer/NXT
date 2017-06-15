using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXT.Services;
using Xamarin.Forms;

namespace NXT.ViewModels
{
    public class MainViewModel
    {
        public ServiceApi ServiceApi { get; private set; }

        private Dictionary<Guid, String> m_GroupColourDictionary = new Dictionary<Guid, string>();
        public Dictionary<Guid, string> GroupColourDictionary
        {
            get
            {
                return m_GroupColourDictionary;
            }
            set
            {
                m_GroupColourDictionary = value;
            }
        }
        public List<string> Colours = new List<string>() {
                "#c62828",//red
                "#ad1457",//pink
                "#6a1b9a",//purple
                "#4527a0",//deep purple
                "#283593",//indigo
                "#1565c0",//blue
                "#0277bd",//l blue
                "#00838f",//cyyan
                "#00695c",//teal
                "#2e7d32",//green
                "#558b2f",//l green
                "#9e9d24",//yello
                "#f9a825",//lime
                "#ff8f00",//amber
                "#ef6c00",//orange
                "#d84315",//deep orange
                "#4e342e",//Brown
                "#424242",//Grey
                "#37474f",//BlueGrey
            };

        public List<FileImageSource> Icons = new List<FileImageSource>()
        {
              (FileImageSource)ImageSource.FromFile("ic_currency_usd_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_currency_gbp_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_currency_btc_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_coffee_outline_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_food_croissant_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_car_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_hotel_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_hamburger_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_food_fork_drink_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_delete_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_cup_white_48dp.png"),
              (FileImageSource)ImageSource.FromFile("ic_cat_white_48dp.png"),
        };

        public MainViewModel()
        {
            ServiceApi = new ServiceApi();
            //GroupColourDictionary = new Dictionary<Guid, string>();

        }

        private static string ColoursFileName()
        {
            return "groupcolours.json";
        }

        public async Task RefreshGroupColours()
        {
            var text = await DependencyService.Get<ISaveAndLoad>().ReadAllTextAsync(ColoursFileName());
            var dic = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(text);
            if (dic != null)
            {
                GroupColourDictionary = dic;
            }
            //var jsonPath = Common.LocalData.InProgressFileName();

            //if (System.IO.File.Exists(jsonPath))
            //{
            //    try
            //    {
            //        var jsonData = await Common.FileStorageAdapter.LoadDataAsync(jsonPath);
            //        InProgressAnswerFiles = JsonConvert.DeserializeObject<IList<Infiniti.Model.Apis.AnswerFile>>(jsonData);
            //    }
            //    catch (Exception ex)
            //    {
            //        m_Log.ErrorException(this.GetType(), "Error loading " + jsonPath, ex);
            //    }
            //}

            //if (InProgressAnswerFiles == null)
            //{
            //    InProgressAnswerFiles = new List<Infiniti.Model.Apis.AnswerFile>();
            //}
        }

        public async Task SaveGroupColours()
        {
            string obj = JsonConvert.SerializeObject(GroupColourDictionary);
            await DependencyService.Get<ISaveAndLoad>().WriteAllTextAsync(ColoursFileName(), obj);
        }

        public string RandomColour()
        {
            Random r = new Random();
            return Colours[r.Next(19)];
        }

    }
}
