//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZonePlayer;

namespace AllInOnePlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Define the name of the default playlist in master playlist
        /// </summary>
        private static string NameDefaultPlaylist = Properties.Settings.Default.NameDefaultPlaylist;

        /// <summary>
        /// Define the name of the channel playlist in master playlist
        /// </summary>
        private static string NameChannelPlaylist = Properties.Settings.Default.NameChannelPlaylist;
        
        /// <summary>
        /// Define the name of the home control playlist in master playlist
        /// </summary>
        private static string NameHomeControlPlaylist = Properties.Settings.Default.NameHomeControlPlaylist;

        /// <summary>
        /// Define the background color of the button images for channels
        /// </summary>
        private static string ChannelImageBackground = Properties.Settings.Default.ChannelImageBackground;

        /// <summary>
        /// Define the height of the button images for channels
        /// </summary>
        private static int ChannelImageSizeHeight = Properties.Settings.Default.ChannelImageSizeHeight;

        /// <summary>
        /// Define hte width of the button images for channels
        /// </summary>
        private static int ChannelImageSizeWidth = Properties.Settings.Default.ChannelImageSizeWidth;

        /// <summary>
        /// Define the background color of the button images for home control
        /// </summary>
        private static string HomeControlImageBackground = Properties.Settings.Default.HomeControlImageBackground;

        /// <summary>
        /// Define hte height of the button images for home control
        /// </summary>
        private static int HomeControlImageSizeHeight = Properties.Settings.Default.HomeControlImageSizeHeight;

        /// <summary>
        /// Define hte width of the button images for home control
        /// </summary>
        private static int HomeControlImageSizeWidth = Properties.Settings.Default.HomeControlImageSizeWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the application and load the playlists
            LoadMasterPlaylist();
            InitializeChannels();
            InitializeHomeControl();

            // Setup of the default play channel
            InitializeDefaultPlayer();
        }

        /// <summary>
        /// Load the master playlist and find the three playlists used to render the gui
        /// default will be default playlist that starts playing
        /// channels will render all channels as selectable buttons
        /// homecontrol will be rendered in a browser control
        /// </summary>
        private void LoadMasterPlaylist()
        {
            // Get master playlist from configuration
            string master = Properties.Settings.Default.MasterPlaylist;
            if (string.IsNullOrWhiteSpace(master))
            {
                throw new Exception("Master playlist not found in AllInOnePlayer.exe.config");
            }

            this.MasterPlaylist = PlaylistManager.Create(new Uri(this.AbsolutePaths(master)), true, "master", false);

            // Get default playlist from master playlist
            ZonePlaylist list;
            this.TryGetPlaylistItem(this.MasterPlaylist.PlayList.Where(playList => playList.ItemName.ToLower().CompareTo(NameDefaultPlaylist) == 0).FirstOrDefault(), out list);
            this.DefaultPlaylist = list;

            // Get channel playlist from master playlist
            this.TryGetPlaylistItem(this.MasterPlaylist.PlayList.Where(playList => playList.ItemName.ToLower().CompareTo(NameChannelPlaylist) == 0).FirstOrDefault(), out list);
            this.ChannelPlaylist = list;

            // Get homecontrol playlist from master playlist
            this.TryGetPlaylistItem(this.MasterPlaylist.PlayList.Where(playList => playList.ItemName.ToLower().CompareTo(NameHomeControlPlaylist) == 0).FirstOrDefault(), out list);
            this.HomeControlPlaylist = list;
        }

        /// <summary>
        /// Start playing the defult channel. This is first channel in default playlist
        /// </summary>
        private void InitializeDefaultPlayer()
        {
            if (this.DefaultPlaylist != null)
            {
                ZonePlaylistItem item = this.DefaultPlaylist.PlayList.FirstOrDefault();
                if (item != null)
                {
                    this.Play(item);
                }
            }
        }

        /// <summary>
        /// Initialize the channel controls. This definitions is found in the channel playlist and needs images and corresponding uri's.
        /// The channel will start playing when the image is pressed.
        /// </summary>
        private void InitializeChannels()
        {
            if (this.ChannelPlaylist != null)
            {
                // Setup channel buttons
                List<Uri> banners = this.ChannelPlaylist.PlayList.Select(item => item.BannerUri).ToList();
                for (int inx = 0; inx < banners.Count; inx++)
                {
                    ListBoxItem item = CreateButtonInListBox(ChannelImageBackground, banners[inx], inx, ChannelImageSizeHeight, ChannelImageSizeWidth, ChannelButton_Click);
                    item.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    if (item != null)
                    {
                        channelList.Children.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Create a button
        /// </summary>
        /// <param name="banner">Image for the button</param>
        /// <param name="index">Index in the list of channels</param>
        /// <param name="imageHeight">Height of image</param>
        /// <param name="imageWidth">Width of the image</param>
        /// <returns></returns>
        private ListBoxItem CreateButtonInListBox(string background, Uri banner, int index, int imageHeight, int imageWidth, RoutedEventHandler callBack)
        {
            if (banner == null)
            {
                return null;
            }

            Button button = this.CreateButton(background, banner, index, imageHeight, imageWidth, callBack);
            ListBoxItem item = new ListBoxItem();
            item.Content = button;
            return item;
        }


        /// <summary>
        /// Create a button
        /// </summary>
        /// <param name="banner">Image for the button</param>
        /// <param name="index">Index in the list of channels</param>
        /// <param name="imageHeight">Height of image</param>
        /// <param name="imageWidth">Width of the image</param>
        /// <returns></returns>
        private Button CreateButton(string background, Uri banner, int index, int imageHeight, int imageWidth, RoutedEventHandler callBack)
        {
            if (banner == null)
            {
                return null;
            }

            // Get image for button
            Image img = new Image();
            img.Source = new BitmapImage(banner);
            img.Height = img.Width = 80;
            img.Height = img.Width = 80;

            // Create button
            Button button = new Button();
            button.Content = img;
            button.Name = string.Format("button{0:0}", index);
            button.CommandParameter = index;
            button.Click += (RoutedEventHandler)callBack;
            button.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(background));
            return button;
        }

        /// <summary>
        /// Setup home control browser item
        /// </summary>
        private void InitializeHomeControl()
        {
            if (this.HomeControlPlaylist != null)
            {
                // Setup home control buttons
                List<Uri> banners = this.HomeControlPlaylist.PlayList.Select(element => element.BannerUri).ToList();
                if (banners. Count > 0)
                {
                    // Add buttons
                    for (int inx = 0; inx < banners.Count; inx++)
                    {
                        Button item = CreateButton(HomeControlImageBackground, banners[inx], inx, HomeControlImageSizeHeight, HomeControlImageSizeWidth, HomeControlButton_Click);
                        if (item != null)
                        {
                            homeControlList.Children.Add(item);
                        }
                    }
                }

                // Setup browser control
                ZonePlaylistItem homeControl = this.HomeControlPlaylist.PlayList.FirstOrDefault();
                this.NavigateHomeControl(homeControl);
            }
        }

        /// <summary>
        /// Navigate browser to a new page
        /// </summary>
        /// <param name="item">Item from the playlist</param>
        private void NavigateHomeControl(ZonePlaylistItem item)
        {
            if (item != null && item.ItemUri != null)
            {
                browser.Dispatcher.BeginInvoke((Action)(() =>
                {
                    browser.Navigate(item.ItemUri);
                }));
            }
        }

        /// <summary>
        /// Try to get item from the playlist
        /// </summary>
        /// <param name="item">Reference to a playlist</param>
        /// <param name="zonePlaylist">The playlist</param>
        /// <returns>True if the item is found</returns>
        private bool TryGetPlaylistItem(ZonePlaylistItem item, out ZonePlaylist zonePlaylist)
        {
            // Try to open playlist item
            try
            {
                zonePlaylist = this.GetPlaylistItem(item);
                return true;
            }
            catch(Exception)
            {
                zonePlaylist = null;
                return false;
            }
        }

        /// <summary>
        /// Get item from the playlist
        /// </summary>
        /// <param name="item">Reference to a playlist</param>
        /// <returns>The playlist</returns>
        private ZonePlaylist GetPlaylistItem(ZonePlaylistItem item)
        {
            if (item == null)
            {
                // Playlist item does not exist
                return null;
            }

            return PlaylistManager.Create(item.ItemUri, true, item.ItemName);
        }

        /// <summary>
        /// Play and item from the playlist.
        /// If the playlist item define a specific player, that player will be selected
        /// </summary>
        /// <param name="item">Item to play</param>
        private void Play(ZonePlaylistItem item)
        {
            if (this.Player != null)
            {
                this.Player.Stop();
            }

            this.Player = PlayerManager.Create(PlayerType.axVlc, playerHost);
            this.Player.Play(item);
        }

        /// <summary>
        /// Start playing the item in the channel playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        void ChannelButton_Click(object sender, RoutedEventArgs e)
        {
            int bannerIndex = (int)(sender as Button).CommandParameter;
            this.Play(this.ChannelPlaylist.PlayList[bannerIndex]);
        }

        /// <summary>
        /// Home control button clicked
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        void HomeControlButton_Click(object sender, RoutedEventArgs e)
        {
            int bannerIndex = (int)(sender as Button).CommandParameter;
            ZonePlaylistItem selection = this.HomeControlPlaylist.PlayList[bannerIndex];
            this.NavigateHomeControl(selection);
        }


        /// <summary>
        /// Convert relative paths into absolute paths for the playlists
        /// </summary>
        /// <param name="playlist">Paths to playlist</param>
        /// <returns></returns>
        private string AbsolutePaths(string playlist)
        {
            string outPath = Directory.GetCurrentDirectory();
            string path = playlist.Trim();
            if (path.StartsWith(".\\"))
            {
                return outPath + path.Substring(1);
            }
            else
            {
                return path;
            }
        }


        /// <summary>
        /// Gets or sets the current player
        /// </summary>
        private ZonePlayer.ZonePlayer Player
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default playlist
        /// </summary>
        private ZonePlaylist DefaultPlaylist
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HomeControl playlist
        /// </summary>
        private ZonePlaylist HomeControlPlaylist
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the channel playlist
        /// </summary>
        private ZonePlaylist ChannelPlaylist
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HomeControl playlist
        /// </summary>
        private ZonePlaylist MasterPlaylist
        {
            get;
            set;
        }
    }
}
