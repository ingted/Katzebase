using System.Drawing.Drawing2D;

namespace NTDLS.WinFormsHelpers.Controls
{
    /// <summary>
    /// An infinitely spinning activity indicator control
    /// </summary>
    public partial class ActivityIndicator : UserControl
    {
        private Color _inactiveColor = Color.FromArgb(218, 218, 218);
        private Color _activeColor = Color.FromArgb(35, 146, 33);
        private Color transitionColor = Color.FromArgb(129, 242, 121);

        private Region? _innerBackgroundRegion;
        private GraphicsPath[] _segmentPaths = new GraphicsPath[12];

        private bool _autoIncrement = true;
        private double _incrementFrequency = 100;
        private bool _behindIsActive = true;
        private int _transitionSegment = 0;

        private System.Timers.Timer? _autoRotateTimer = null;

        /// <summary>
        /// Creates a new instance of the spinning activity control.
        /// </summary>
        public ActivityIndicator()
        {
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            CalculateSegments();
            AutoIncrementFrequency = 100;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            AutoIncrement = true;
        }

        /// <summary>
        /// The color of the inactive segment.
        /// </summary>
        public Color InactiveSegmentColor
        {
            get
            {
                return _inactiveColor;
            }
            set
            {
                _inactiveColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The color of the active segment.
        /// </summary>
        public Color ActiveSegmentColor
        {
            get
            {
                return _activeColor;
            }
            set
            {
                _activeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The color of the transition segment.
        /// </summary>
        public Color TransitionSegmentColor
        {
            get
            {
                return transitionColor;
            }
            set
            {
                transitionColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Whether the inactive portion of the control should appear active.
        /// </summary>
        public bool BehindTransitionSegmentIsActive
        {
            get
            {
                return _behindIsActive;
            }
            set
            {
                _behindIsActive = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the current transition segment.
        /// </summary>
        public int TransitionSegment
        {
            get
            {
                return _transitionSegment;
            }
            set
            {
                if (value > 12 || value < -1)
                {
                    throw new ArgumentException("TransitionSegment must be between -1 and 12");
                }
                _transitionSegment = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Whether the control should auto-update or not.
        /// </summary>
        public bool AutoIncrement
        {
            get
            {
                return _autoIncrement;
            }
            set
            {
                _autoIncrement = value;

                if (value == false && _autoRotateTimer != null)
                {
                    _autoRotateTimer.Dispose();
                    _autoRotateTimer = null;
                }

                if (value == true && _autoRotateTimer == null)
                {
                    _autoRotateTimer = new System.Timers.Timer(_incrementFrequency);

                    _autoRotateTimer.Elapsed += IncrementTransitionSegment;
                    _autoRotateTimer.Start();
                }
            }
        }

        /// <summary>
        /// The frequency in miliseconds that the control updates.
        /// </summary>
        public double AutoIncrementFrequency
        {
            get
            {
                return _incrementFrequency;
            }
            set
            {
                _incrementFrequency = value;

                if (_autoRotateTimer != null)
                {
                    AutoIncrement = false;
                    AutoIncrement = true;
                }
            }
        }

        private void CalculateSegments()
        {
            Rectangle rctFull = new(0, 0, Width, Height);
            Rectangle rctInner = new((int)(Width * (7.0 / 30.0)),
                                                (int)(Height * (7.0 / 30.0)),
                                                (int)(Width - (Width * (7.0 / 30.0) * 2.0)),
                                                (int)(Height - (Height * (7.0 / 30.0) * 2.0)));
            GraphicsPath pthInnerBackground;

            //Create 12 segment pieces
            for (int intCount = 0; intCount < 12; intCount++)
            {
                _segmentPaths[intCount] = new GraphicsPath();

                //We subtract 90 so that the starting segment is at 12 o'clock
                _segmentPaths[intCount].AddPie(rctFull, (intCount * 30) - 90, 25);
            }

            //Create the center circle cut-out
            pthInnerBackground = new GraphicsPath();
            pthInnerBackground.AddPie(rctInner, 0, 360);
            _innerBackgroundRegion = new Region(pthInnerBackground);
        }

        private void SpinningProgress_EnabledChanged(object sender, System.EventArgs e)
        {
            if (Enabled)
            {
                _autoRotateTimer?.Start();
            }
            else
            {
                _autoRotateTimer?.Stop();
            }
        }

        private void IncrementTransitionSegment(object? sender, System.Timers.ElapsedEventArgs? e)
        {
            if (_transitionSegment == 12)
            {
                _transitionSegment = 0;
                _behindIsActive = !_behindIsActive;
            }
            else if (_transitionSegment == -1)
            {
                _transitionSegment = 0;
            }
            else
            {
                _transitionSegment += 1;
            }
            Invalidate();
        }

        private void ProgressDisk_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (_innerBackgroundRegion != null)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.ExcludeClip(_innerBackgroundRegion);

                for (int intCount = 0; intCount < 12; intCount++)
                {
                    if (Enabled)
                    {
                        if (intCount == _transitionSegment)
                        {
                            //If this segment is the transition segment, color it differently
                            e.Graphics.FillPath(new SolidBrush(transitionColor), _segmentPaths[intCount]);
                        }
                        else if (intCount < _transitionSegment)
                        {
                            //This segment is behind the transition segment
                            if (_behindIsActive)
                            {
                                //If behind the transition should be active, 
                                //color it with the active color
                                e.Graphics.FillPath(new SolidBrush(_activeColor), _segmentPaths[intCount]);
                            }
                            else
                            {
                                //If behind the transition should be in-active, 
                                //color it with the in-active color
                                e.Graphics.FillPath(new SolidBrush(_inactiveColor), _segmentPaths[intCount]);
                            }
                        }
                        else
                        {
                            //This segment is ahead of the transition segment
                            if (_behindIsActive)
                            {
                                //If behind the the transition should be active, 
                                //color it with the in-active color
                                e.Graphics.FillPath(new SolidBrush(_inactiveColor), _segmentPaths[intCount]);
                            }
                            else
                            {
                                //If behind the the transition should be in-active, 
                                //color it with the active color
                                e.Graphics.FillPath(new SolidBrush(_activeColor), _segmentPaths[intCount]);
                            }
                        }
                    }
                    else
                    {
                        //Draw all segments in in-active color if not enabled
                        e.Graphics.FillPath(new SolidBrush(_inactiveColor), _segmentPaths[intCount]);
                    }
                }
            }
        }

        private void ProgressDisk_Resize(object sender, System.EventArgs e)
        {
            CalculateSegments();
        }

        private void ProgressDisk_SizeChanged(object sender, System.EventArgs e)
        {
            CalculateSegments();
        }
    }
}
