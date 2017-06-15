using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXT.Controls;
using NXT.Views;
using Xamarin.Forms;

namespace NXT.Controls
{

    public class GridListView : Grid
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(GridListView), null, BindingMode.OneWay, null, ItemsChanged);
        //public static readonly BindableProperty ParentVMProperty = BindableProperty.Create(nameof(ParentVM), typeof(object), typeof(object), null, BindingMode.OneWay);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(GridListView), null);
        //public static readonly BindableProperty MaxColumnsProperty = BindableProperty.Create(nameof(MaxColumns), typeof(int), typeof(int), (object)2, BindingMode.OneWay);
        //
        
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GridListView), null);

        //public static readonly BindableProperty ItemsSourceProperty = 
        //BindableProperty.Create<GridView, IEnumerable<object>>(p => p.ItemsSource, null, BindingMode.OneWay, null, (bindable, oldValue, newValue) => { ((GridView)bindable).BuildTiles(newValue); });

        public GridListView()
        {
        }

        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private int m_MaxColumns = 2;
        public int MaxColumns
        {
            get
            {
                return m_MaxColumns;
            }
            set
            {
                m_MaxColumns = value;
                this.ColumnDefinitions.Clear();
                for (int i = 0; i < (int)this.m_MaxColumns; i++)
                {
                    var gridLength = new GridLength(((1.0 / (int)MaxColumns)), GridUnitType.Star);
                    var colDef = new ColumnDefinition() { Width = gridLength };
                    this.ColumnDefinitions.Add(colDef);
                }
            }
        }
        //{
        //    get { return GetValue(MaxColumnsProperty); }
        //    set { SetValue(MaxColumnsProperty, value); }
        //}

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void ItemsChanged(
                            BindableObject bindable,
                            object oldValue,
                            object newValue)
        {
            ((GridListView)bindable).BuildTiles();
        }

        public void BuildTiles()
        {
            Children?.Clear();
            RowDefinitions?.Clear();

            if (ItemsSource != null)
            {
                var items = ItemsSource as IList ?? ItemsSource.ToList();
                //var enumerable = tiles as IList ?? tiles.ToList();
                var numberOfRows = Math.Ceiling(((double)items.Count / (int)MaxColumns));

                for (var i = 0; i < numberOfRows; i++)
                {
                    RowDefinitions?.Add(new RowDefinition() { Height = new GridLength(1.0, GridUnitType.Auto) });
                }

                for (var index = 0; index < items.Count; index++)
                {
                    var column = index % (int)MaxColumns;
                    var row = (int)Math.Floor(((double)index / (int)MaxColumns));

                    //var tile = BuildTile(items[index]);
                    var tile = items[index];

                    //var buildTile = (Layout)Activator.CreateInstance(typeof(GridItemTemplate), tile);
                    //buildTile.BackgroundColor = Color.Transparent;
                    //buildTile.InputTransparent = false;

                    //var tapGestureRecognizer = new TapGestureRecognizer
                    //{
                    //    Command = Command,
                    //    CommandParameter = tile,
                    //    NumberOfTapsRequired = 1,
                    //};

                    //buildTile?.GestureRecognizers.Add(tapGestureRecognizer);
                    Button b = null;
                    if (tile is string)
                    {
                        b = new Button()
                        {
                            BackgroundColor = Color.FromHex((string)items[index]),
                            Command = Command,
                            CommandParameter = index,
                            BorderRadius = 5,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        };
                        //Children?.Add(buildTile, column, row);
                        Children?.Add(b, column, row);
                    }
                    else if (tile is FileImageSource)
                    {
                        CustomImageButton imageButton = new CustomImageButton()
                        {
                            Image = ((FileImageSource)items[index]),
                            BackgroundColor = Color.Transparent,
                            Command = Command,
                            CommandParameter = tile,
                            BorderRadius = 5,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        };
                        Children?.Add(imageButton, column, row);

                    }

                }
                this.BackgroundColor = Color.Transparent;
                this.InputTransparent = true;
                //var tapGestureRecognizer = new TapGestureRecognizer
                //{
                //    Command = Command,
                //    CommandParameter = "123",
                //    NumberOfTapsRequired = 1,

                //};
                //this.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        private Layout BuildTile(object item1)
        {
            var buildTile = (Layout)Activator.CreateInstance(typeof(GridItemTemplate), item1);
            buildTile.InputTransparent = false;

            var tapGestureRecognizer = new TapGestureRecognizer
            {
                Command = Command,
                CommandParameter = item1,
                NumberOfTapsRequired = 1,
            };

            buildTile?.GestureRecognizers.Add(tapGestureRecognizer);


            return buildTile;
        }
    }
}