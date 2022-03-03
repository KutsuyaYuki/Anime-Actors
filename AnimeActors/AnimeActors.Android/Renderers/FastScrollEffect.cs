using Android.Content;
using Android.Graphics;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using AnimeActors.Droid.Renderers;
using AnimeActors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static AndroidX.RecyclerView.Widget.RecyclerView;

[assembly: ExportRenderer(typeof(NativeCollectionView), typeof(NativeAndroidCollectionViewRenderer))]
namespace AnimeActors.Droid.Renderers
{
    public class NativeAndroidCollectionViewRenderer : CollectionViewRenderer
    {
        public const int INDWIDTH = 25;
        public const int INDHEIGHT = 18;
        public float ScaledWidth { get; set; }
        public float ScaledHeight { get; set; }
        public string[] Sections { get; set; }
        public float Sx { get; set; }
        public float Sy { get; set; }
        public string Section { get; set; }
        public bool ShowLetter { get; set; }

        private ListHandler _listHandler;
        private bool _setupThings = false;
        private Context _context;

        public NativeAndroidCollectionViewRenderer(Context context) : base(context)
        {
            _context = context;
            
            FastScrollRecyclerViewItemDecoration decoration = new FastScrollRecyclerViewItemDecoration(context);
            this.AddItemDecoration(decoration);
        }
        public override void OnDraw(Canvas c)
        {
            if (!_setupThings && GetAdapter() != null)
                SetupThings();
            base.OnDraw(c);
        }

        private Dictionary<string, int> GetMapIndex()
        {
            return ItemsView
                .ItemsSource?
                .OfType<IDefineJumpKey>()
                .Select((c, i) => new { key = c.JumpKey[..1].ToUpper(), val = i })
                .GroupBy(x => x.key)
                .Select(x => x.First())
                .ToDictionary(c => c.key, c => c.val);
        }

        private void SetupThings()
        {
            //create az text data
            var sectionSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(c => c.ToString());
            var listSection = new List<string>(sectionSet);
            listSection.Sort();
            Sections = new string[listSection.Count];
            int i = 0;
            foreach (var s in listSection)
            {
                Sections[i++] = s;
            }

            ScaledWidth = INDWIDTH * _context.Resources.DisplayMetrics.Density;
            var divisor = sectionSet.Count() == 0 ? 1 : sectionSet.Count();
            ScaledHeight = Height / divisor;
            Sx = Width - PaddingRight - (float)(1.2 * ScaledWidth);
            Sy = (float)((Height - (ScaledHeight * Sections.Length)) / 2.0);
            _setupThings = true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!_setupThings)
            {
                return base.OnTouchEvent(e);
            }

            var x = e.GetX();
            var y = e.GetY();

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    {
                        // Outside our area of expertise
                        if (x < Sx - ScaledWidth || y < Sy || y > (Sy + ScaledHeight * Sections.Length))
                        {
                            return base.OnTouchEvent(e);
                        }
                        // We touched the index bar
                        float yy = y - PaddingTop - PaddingBottom - Sy;
                        int currentPosition = (int)Math.Floor(yy / ScaledHeight);
                        if (currentPosition < 0) currentPosition = 0;
                        if (currentPosition >= Sections.Length) currentPosition = Sections.Length - 1;
                        Section = Sections[currentPosition];
                        ShowLetter = true;
                        int positionInData = 0;
                        if (GetMapIndex().ContainsKey(Section.ToUpper()))
                        {
                            positionInData = GetMapIndex()[Section.ToUpper()];
                        }

                        (GetLayoutManager() as LinearLayoutManager).ScrollToPositionWithOffset(positionInData, 20);
                        Invalidate();
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        // Outside our area of expertise
                        if (!ShowLetter && (x < Sx - ScaledWidth || y < Sy || y > (Sy + ScaledHeight * Sections.Length)))
                        {
                            return base.OnTouchEvent(e);
                        }

                        // We are moving over the index bar
                        float yy = y - Sy;
                        int currentPosition = (int)Math.Floor(yy / ScaledHeight);
                        if (currentPosition < 0) currentPosition = 0;
                        if (currentPosition >= Sections.Length) currentPosition = Sections.Length - 1;
                        Section = Sections[currentPosition];
                        ShowLetter = true;
                        int positionInData = 0;
                        if (GetMapIndex().ContainsKey(Section.ToUpper()))
                            positionInData = GetMapIndex()[Section.ToUpper()];
                        (GetLayoutManager() as LinearLayoutManager).ScrollToPositionWithOffset(positionInData, 20);
                        Invalidate();
                        break;
                    }
                case MotionEventActions.Up:
                    {
                        _listHandler = new ListHandler(this);
                        _listHandler.DelayClear();
                        return base.OnTouchEvent(e);
                    }
            }

            return base.OnTouchEvent(e);
        }


        private class ListHandler
        {
            NativeAndroidCollectionViewRenderer _parent;
            public ListHandler(NativeAndroidCollectionViewRenderer parent)
            {
                _parent = parent;
            }

            public async void DelayClear()
            {
                await Task.Delay(100);
                _parent.ShowLetter = false;
                _parent.Invalidate();
            }
        }
    }

    public class FastScrollRecyclerViewItemDecoration : ItemDecoration
    {
        private readonly Context _context;
        public FastScrollRecyclerViewItemDecoration(Context context)
        {
            _context = context;
        }

        public override void OnDrawOver(Canvas canvas, RecyclerView parent, State state)
        {
            base.OnDrawOver(canvas, parent, state);

            NativeAndroidCollectionViewRenderer parentRenderer = parent as NativeAndroidCollectionViewRenderer;
            float scaledWidth = parentRenderer.ScaledWidth;
            float sx = parentRenderer.Sx;
            float scaledHeight = parentRenderer.ScaledHeight;
            float sy = parentRenderer.Sy;
            string[] sections = parentRenderer.Sections;
            string section = parentRenderer.Section;
            bool showLetter = parentRenderer.ShowLetter;

            // draw index A-Z
            Paint textPaint = new Paint
            {
                AntiAlias = true
            };
            textPaint.SetStyle(Paint.Style.Fill);

            for (int i = 0; i < sections.Length; i++)
            {
                if (showLetter & section != null && !section.Equals("") && section != null
                        && sections[i].ToUpper().Equals(section.ToUpper()))
                {
                    textPaint.Color = Android.Graphics.Color.White;
                    textPaint.Alpha = 255;
                    textPaint.FakeBoldText = true;
                    textPaint.TextSize = scaledWidth / 2;
                    canvas.DrawText(sections[i].ToUpper(),
                            sx + textPaint.TextSize / 2, sy + parent.PaddingTop
                                    + scaledHeight * (i + 1), textPaint);
                    textPaint.TextSize = scaledWidth;
                    float middleTextSize = 100;
                    Paint middleLetter = new Paint
                    {
                        Color = new Android.Graphics.Color(ContextCompat.GetColor(_context, Resource.Color.colorPrimary)),
                        TextSize = middleTextSize,
                        AntiAlias = true,
                        FakeBoldText = true
                    };
                    middleLetter.SetStyle(Paint.Style.Fill);

                    var x = sx - textPaint.TextSize - 80;
                    var y = sy + parent.PaddingTop + scaledHeight * (i + 1) + scaledHeight / 3 - 100;

                    RectF bubble = new RectF();

                    bubble.Set(x - 30, y - 100, x + 120, y + 50);

                    var corners = new float[]{
                        80, 80,        // Top left radius in px
                        80, 80,        // Top right radius in px
                        0, 0,          // Bottom right radius in px
                        80, 80         // Bottom left radius in px
                    };

                    var path = new Path();
                    path.AddRoundRect(bubble, corners, Path.Direction.Cw);

                    canvas.DrawPath(path, textPaint);
                    canvas.DrawText(section.ToUpper(), x, y, middleLetter);

                }
                else
                {
                    textPaint.Color = Android.Graphics.Color.Red;
                    textPaint.Alpha = 200;
                    textPaint.FakeBoldText = false;
                    textPaint.TextSize = scaledWidth / 2;
                    canvas.DrawText(sections[i].ToUpper(),
                            sx + textPaint.TextSize / 2, sy + parent.PaddingTop
                                    + scaledHeight * (i + 1), textPaint);
                }
            }
        }
    }
}