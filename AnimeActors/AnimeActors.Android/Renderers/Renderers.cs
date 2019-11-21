using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AnimeActors.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SearchBar), typeof(CustomSearchBarRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(CustomTextCellRenderer.SuperEntryRenderer))]
[assembly: ExportRenderer(typeof(SearchBar), typeof(CustomTextCellRenderer.StyledSearchBarRenderer))]

namespace AnimeActors.Droid.Renderers
{
    public class CustomSearchBarRenderer : SearchBarRenderer
    {
        public CustomSearchBarRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                LinearLayout linearLayout = this.Control.GetChildAt(0) as LinearLayout;
                linearLayout = linearLayout.GetChildAt(2) as LinearLayout;
                linearLayout = linearLayout.GetChildAt(1) as LinearLayout;

                linearLayout.Background = null; //removes underline

                AutoCompleteTextView textView = linearLayout.GetChildAt(0) as AutoCompleteTextView; //modify for text appearance customization 
            }
        }
    }

    class Renderers
    {

    }

    public class CustomTextCellRenderer : TextCellRenderer
    {
        public Android.Views.View cellCore;
        public Drawable unselectedBackground;
        public bool selected;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent,
            Context context)
        {
            cellCore = base.GetCellCore(item, convertView, parent, context);

            // Save original background to rollback to it when not selected,
            // We assume that no cells will be selected on creation.
            selected = false;
            unselectedBackground = cellCore.Background;

            return cellCore;
        }

        /// <summary>
        /// This class gets rid of the magnifying icon in the SearchBar.
        /// </summary>
        public class StyledSearchBarRenderer : SearchBarRenderer
        {
            public StyledSearchBarRenderer(Context context) : base(context)
            {
            }

            protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
            {
                base.OnElementChanged(e);
                if (Control != null)
                {
                    var searchView = base.Control as SearchView;

                    //Get the Id for your search icon
                    int searchIconId = Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
                    ImageView searchViewIcon = (ImageView) searchView.FindViewById<ImageView>(searchIconId);
                    ViewGroup linearLayoutSearchView = (ViewGroup) searchViewIcon.Parent;

                    searchViewIcon.SetAdjustViewBounds(true);

                    //Remove the search icon from the view group (if we add it again it'll be at the end of the searchbar).
                    linearLayoutSearchView.RemoveView(searchViewIcon);
                }
            }
        }

        public class ElementRenderer : VisualElementRenderer<Xamarin.Forms.View>
        {
            public ElementRenderer(Context context) : base(context)
            {
            }
        }

        public class SuperEntryRenderer : EntryRenderer
        {
            public SuperEntryRenderer(Context context) : base(context)
            {
            }

            protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
            {

                base.OnElementChanged(e);
                if (e.OldElement == null)
                {
                    var nativeEditText = (EditText) Control;
                    var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RectShape());
                    shape.Paint.Color = Xamarin.Forms.Color.Red.ToAndroid();
                    shape.Paint.SetStyle(Paint.Style.Stroke);
                    nativeEditText.Background = shape;
                }
            }
        }
    }
}