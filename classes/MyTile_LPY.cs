using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using ESRI.ArcGIS.Client;
using System.IO;
using System.Runtime.Serialization.Json;
using ESRI.ArcGIS.Client.Geometry;
using System.Data.SQLite;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;

using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;

namespace zsdpmap
{
    /// <summary>
    /// LPY 2014-12-28 10:36:12 修改 更改瓦片图写入SQLite数据库的方式
    /// </summary>
    class MyTile : TiledMapServiceLayer //TiledLayer
    {
        #region Property & Fields
        public static readonly DependencyProperty EnableOfflineProperty =
            DependencyProperty.Register("EnableOffline", typeof(bool), typeof(MyTile), new PropertyMetadata(true));
        /// <summary>
        /// Whether to allow this layer works in offline mode. If false, it is an original ArcGISTiledMapServiceLayer(DeleteOfflineCache property will also take no effect).
        /// Default is true.
        /// </summary>
        public bool EnableOffline
        {
            get { return (bool)GetValue(EnableOfflineProperty); }
            set { SetValue(EnableOfflineProperty, value); }
        }

        public static readonly DependencyProperty SaveOfflineTilesProperty =
            DependencyProperty.Register("SaveOfflineTiles", typeof(bool), typeof(MyTile), new PropertyMetadata(true));
        /// <summary>
        /// Whether to switch on the auto saving downloaded tiles to local sqlite db ability.
        /// Default is true.
        /// Only take effect when LoadOfflineTileFirst==false
        /// </summary>
        public bool SaveOfflineTiles
        {
            get { return (bool)GetValue(SaveOfflineTilesProperty); }
            set { SetValue(SaveOfflineTilesProperty, value); }
        }

        public static readonly DependencyProperty LoadOfflineTileFirstProperty =
            DependencyProperty.Register("LoadOfflineTileFirst", typeof(bool), typeof(MyTile), new PropertyMetadata(false));
        /// <summary>
        /// If true, this layer will load offline db tile first.
        /// If false, this layer will load online map service tile first.
        /// Default is false.
        /// </summary>
        public bool LoadOfflineTileFirst
        {
            get { return (bool)GetValue(LoadOfflineTileFirstProperty); }
            set { SetValue(LoadOfflineTileFirstProperty, value); }
        }

        public static readonly DependencyProperty DeleteSavedOfflineTilesProperty =
            DependencyProperty.Register("DeleteSavedOfflineTiles", typeof(bool), typeof(MyTile), new PropertyMetadata(false));
        /// <summary>
        /// Whether to delete all offline cache(if has) before initialize. Set this to True only if you want to redownload all cache tiles.
        /// Default is false.
        /// </summary>
        public bool DeleteSavedOfflineTiles
        {
            get { return (bool)GetValue(DeleteSavedOfflineTilesProperty); }
            set { SetValue(DeleteSavedOfflineTilesProperty, value); }
        }

        public enum Mode
        {
            /// <summary>
            /// only save tiles which not exist.
            /// </summary>
            SaveOnly,
            /// <summary>
            /// save tiles which not exist and update tiles which already exist.
            /// </summary>
            SaveOrUpdate
        }
        [TypeConverter(typeof(Mode))]
        public static readonly DependencyProperty SaveTilesModeProperty = DependencyProperty.Register("SaveTilesMode", typeof(Mode), typeof(MyTile), new PropertyMetadata(Mode.SaveOnly));
        /// <summary>
        /// Save offline tiles mode.
        /// Only take effect when LoadOfflineTileFirst==false && SaveOfflineTiles==true
        /// </summary>
        public Mode SaveTilesMode
        {
            get { return (Mode)GetValue(SaveTilesModeProperty); }
            set { SetValue(SaveTilesModeProperty, value); }
        }

        private const string DBName = "OfflineTiles.db";
        private const string TableServicesName = "MapServices";
        private bool _isConnected;
        private SQLiteConnection _conn;
        private List<string> _tilesNeedToSave = new List<string>();
        private List<string> _tilesAlreadySaved = new List<string>();
        private ConcurrentDictionary<string, OneTile> dic = new ConcurrentDictionary<string, OneTile>();
        #endregion

        private string _url = "";
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }
        public override void Initialize()
        {
            this.FullExtent = new ESRI.ArcGIS.Client.Geometry.Envelope(111.925137105232, 9.44664648125718, 141.597475975149, 46.5533847659212)//PGIS
            //this.FullExtent = new ESRI.ArcGIS.Client.Geometry.Envelope(-180, -90, 180, 90)
            {
                SpatialReference = new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326)
            };

            this.SpatialReference = new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326);

            this.TileInfo = new TileInfo()
            {
                Height = 256,
                Width = 256,

                Origin = new ESRI.ArcGIS.Client.Geometry.MapPoint(0, 90)//PGIS
                //Origin = new ESRI.ArcGIS.Client.Geometry.MapPoint(-180, 90)
                {
                    SpatialReference = new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326)
                },
                Lods = new Lod[22]
            };


            double resolution = 2;//PGIS
            //double resolution = 0.703125 * 2;
            for (int i = 0; i < TileInfo.Lods.Length; i++)
            {

                TileInfo.Lods[i] = new Lod() { Resolution = resolution };
                resolution = resolution / 2;
            }
            InitDB();
            InitTileTable();
            base.Initialize();
            base.TileLoaded += new EventHandler<TileLoadEventArgs>(OfflineableTileLayer_TileLoaded);
            //Thread thread = new Thread(new ThreadStart(getImages));//这两行代码用于后台获取瓦片图到数据库，已获取完成，暂时不用，勿删
            //thread.Start();

            //LPY 2014-12-28 11:05:39 添加 创建线程用于遍历ConcurrentDictionary<string, OneTile> dic，如果存在需要写入数据库的瓦片图，则写入
            Thread threadWriteTilesBack = new Thread(new ThreadStart(WriteTiles));
            threadWriteTilesBack.Start();
        }

        //LPY 2014-12-28 10:44:57 添加该方法，改善在线加载瓦片图界面流畅度
        private void WriteTiles()
        {
            OneTile d; // = new OneTile();
            while (true)
            {
                if (dic.Count <= 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                string key = dic.First().Key;
                d = dic.First().Value;
                //    foreach (var d in dic)
                {
                    //WriteSQLite(d.Value);
                    d.e.Position = 0;
                    if (LoadTile(d.level, d.row, d.col)==null)
                    {
                        SaveTile(d.level, d.row, d.col, StreamToBytes(d.e));
                    }
                    
                    // one = d.Value;
                    dic.TryRemove(key, out d);
                }
                //   dic.Clear();
                Thread.Sleep(10);
            }
        }

        public override string GetTileUrl(int level, int row, int col)
        {
            double bound = Math.Pow(2, level);
            double row2 = (bound * 0.17578125 - row - 1);
            string url = _url + "Zoom=" + level + "&Row=" + row2 + "&Col=" + col + "";
            return url;
            //     throw new NotImplementedException();
        }

        protected override void GetTileSource(int level, int row, int col, Action<System.Windows.Media.ImageSource> onComplete)
        {
            //base.GetTileSource(level, row, col, onComplete);
            //return;
            string key = string.Format("{0}/{1}/{2}", level, row, col);
            byte[] tilebytes = null;
            _isConnected = true;
            //_isConnected = NetworkInterface.GetIsNetworkAvailable();
            if (_isConnected)
            {
                if (LoadOfflineTileFirst && ((tilebytes = LoadTile(level, row, col)) != null))
                {
                    if (tilebytes != null)
                    {
                        BitmapImage image = new BitmapImage()
                        {
                            CreateOptions = BitmapCreateOptions.DelayCreation
                        };
                        //using (MemoryStream ms = new MemoryStream(tilebytes, 0, tilebytes.Length))
                        //{
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(tilebytes);
                        image.EndInit();
                        //image.BeginInit();
                        //image.StreamSource = ms; // SetSource(ms);
                        //image.EndInit();
                        //image = ByteArrayToBitmapImage(tilebytes);
                        //}
                        onComplete(image);
                        //mark this tile need to download, so when tile_loaded, avoid to check if tile exist again.
                        if (!_tilesAlreadySaved.Contains(key))
                            _tilesAlreadySaved.Add(key);
                        //Debug.WriteLine(key + " from offline db.");
                    }
                }
                else
                {
                    base.GetTileSource(level, row, col, onComplete);
                    //mark this tile need to download, so when tile_loaded, avoid to check if tile exist again.
                    if (!_tilesAlreadySaved.Contains(key))
                        _tilesNeedToSave.Add(key);
                    //Debug.WriteLine(key + " from online service.");
                }
            }
            else
            {
                tilebytes = LoadTile(level, row, col);
                if (tilebytes != null)
                {
                    BitmapImage image = new BitmapImage()
                    {
                        CreateOptions = BitmapCreateOptions.DelayCreation
                    };
                    //using (MemoryStream ms = new MemoryStream(tilebytes, 0, tilebytes.Length))
                    //{
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(tilebytes);
                    image.EndInit();
                    //image.BeginInit();
                    //image.StreamSource = ms; // SetSource(ms);
                    //image.EndInit();
                    //image = ByteArrayToBitmapImage(tilebytes);
                    //}
                    onComplete(image);
                    //Debug.WriteLine(key + " from offline db.");
                }
                else
                    //base.GetTileSource(level, row, col, onComplete);
                    onComplete(null);
            }
        }

        private void OfflineableTileLayer_TileLoaded(object sender, TiledLayer.TileLoadEventArgs e)
        {
            //This event is only fired if _isConnected==true
            if (EnableOffline && SaveOfflineTiles)
            {
                string key = string.Format("{0}/{1}/{2}", e.Level, e.Row, e.Column);
                if (SaveTilesMode == Mode.SaveOnly && _tilesNeedToSave.Contains(key) && !_tilesAlreadySaved.Contains(key) && e.ImageStream != null)
                {
                    byte[] bytes = LoadTile(e.Level, e.Row, e.Column);
                    if (bytes == null)
                    {
                        //LPY 2014-12-28 10:46:56 修改 先将瓦片图写到内存中，再处理
                        OneTile oneTile = new OneTile(e.Level, e.Row, e.Column, e.ImageStream);
                        try
                        {
                            dic.TryAdd(key, oneTile);
                        }
                        catch (Exception)
                        {
                        }
                        //SaveTile(e.Level, e.Row, e.Column, StreamToBytes(e.ImageStream));
                        //mark this tile already saved, avoid to save it repeatly when LoadOfflineTileFirst=false. 
                        _tilesAlreadySaved.Add(key);
                        //Debug.WriteLine(key + " tile saved.");
                        _tilesNeedToSave.Remove(key);
                    }
                }
                else if (SaveTilesMode == Mode.SaveOrUpdate && e.ImageStream != null)
                {
                    SaveOrReplaceTile(e.Level, e.Row, e.Column, StreamToBytes(e.ImageStream));
                    //  Debug.WriteLine(key + " tile saved or replaced.");
                }
            }
        }

        private void InitDB()
        {
            //IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            //must implement on UI thread to ensure _conn initialized.
            _conn = new SQLiteConnection("Data Source=" + DBName);
            _conn.Open();//will create one in IsolatedStorage if not exist.
            //if (!IsTableExists(TableServicesName))
            //{
            //    //create table
            //    //_conn.BeginTransaction();
            //    SQLiteCommand cmd = _conn.CreateCommand();
            //    cmd.CommandText = @"CREATE TABLE """ + TableServicesName + @""" (""url"" TEXT PRIMARY KEY  NOT NULL  UNIQUE , ""spatialreference"" TEXT NOT NULL , ""fullextent"" TEXT NOT NULL , ""tileinfo"" TEXT NOT NULL )";
            //    cmd.ExecuteNonQuery();
            //    //_conn.CommitTransaction();
            //}
        }

        /// <summary>
        /// check if the database table to store tiles of this service exists.
        /// if not, create one.
        /// </summary>
        private void InitTileTable()
        {
            //check if the tile table exist
            if (!IsTableExists(this.Url))
            {
                SQLiteTransaction Transaction = _conn.BeginTransaction();
                SQLiteCommand cmd = _conn.CreateCommand();
                cmd.CommandText = @"CREATE TABLE """ + this.Url + @""" (""level"" INTEGER NOT NULL , ""row"" INTEGER NOT NULL , ""column"" INTEGER NOT NULL , ""tile"" BLOB NOT NULL )";
                cmd.ExecuteNonQuery();
                cmd = _conn.CreateCommand();
                cmd.CommandText = "CREATE UNIQUE INDEX 'idx_" + this.Url + "' ON '" + this.Url + "' ('level' ASC, 'row' ASC, 'column' ASC)";
                cmd.ExecuteNonQuery();
                Transaction.Commit(); // CommitTransaction();
            }
        }

        /// <summary>
        /// check if the meta data(using to init this layer offline) of this layer stored in db
        /// </summary>
        private bool IsLayerMetaDataExist()
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM " + TableServicesName + " WHERE url='" + this.Url + "'";
            long i = (long)cmd.ExecuteScalar();
            return i == 0 ? false : true;
        }

        /// <summary>
        /// check if the meta data(using to init this layer offline) of this layer stored in db.
        /// if not, store them; if sotred, replace them with fresh values.
        /// </summary>
        private void SaveLayerMetaData(string url, string jsonSR, string jsonFullExtent, string jsonTileInfo)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO '" + TableServicesName + "' (url, spatialreference, fullextent, tileinfo) VALUES ('" + url + "','" + jsonSR + "','" + jsonFullExtent + "','" + jsonTileInfo + "')";
            cmd.ExecuteNonQuery();
        }

        private void LoadLayerMetaData(out SpatialReference sr, out Envelope fullextent, out TileInfo tileinfo)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT * from " + TableServicesName + " WHERE url='" + this.Url + "'";
            //    List<LayerMetaData> list = cmd.ExecuteQuery<LayerMetaData>().ToList<LayerMetaData>();
            SQLiteDataReader reader = cmd.ExecuteReader();
            int count = 0;
            sr = null;
            fullextent = null;
            tileinfo = null;
            if (reader.HasRows)
            {

                while (reader.Read())
                {
                    count++;
                    if (count > 1)
                        break;
                    sr = ConvertJsonStringToObject<SpatialReference>(reader["spatialreference"].ToString());
                    fullextent = ConvertJsonStringToObject<Envelope>(reader["fullextent"].ToString());
                    tileinfo = ConvertJsonStringToObject<TileInfo>(reader["tileinfo"].ToString());

                }
            }
            if (count != 1)
                throw new Exception(TableServicesName + " table contains 0 or more than 1 record.");
            /*
            List<LayerMetaData> list = cmd.ExecuteReader() as List<LayerMetaData> ; 
            if (list.Count != 1)
                throw new Exception(TableServicesName + " table contains 0 or more than 1 record.");
            sr = ConvertJsonStringToObject<SpatialReference>(list[0].spatialreference);
            fullextent = ConvertJsonStringToObject<Envelope>(list[0].fullextent);
            tileinfo = ConvertJsonStringToObject<TileInfo>(list[0].tileinfo);
             */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="tile"></param>
        private void SaveTile(int level, int row, int column, byte[] tile)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "INSERT INTO '" + this.Url + "' (level, row, column,tile) VALUES (@Level,@Row,@Column,@Bytes)";

            cmd.Parameters.Add(new SQLiteParameter("@Level", level));
            cmd.Parameters.Add(new SQLiteParameter("@Row", row));
            cmd.Parameters.Add(new SQLiteParameter("@Column", column));
            cmd.Parameters.Add(new SQLiteParameter("@Bytes", tile));

            cmd.ExecuteNonQuery(); // new Tile(level, row, column, tile));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="tile"></param>
        private void SaveOrReplaceTile(int level, int row, int column, byte[] tile)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO '" + this.Url + "' (level, row, column,tile) VALUES (@Level,@Row,@Column,@Bytes)";
            cmd.Parameters.Add(new SQLiteParameter("@Level", level));
            cmd.Parameters.Add(new SQLiteParameter("@Row", row));
            cmd.Parameters.Add(new SQLiteParameter("@Column", column));
            cmd.Parameters.Add(new SQLiteParameter("@Bytes", tile));
            cmd.ExecuteNonQuery(); // new Tile(level, row, column, tile));
        }

        /// <summary>
        /// 
        /// </summary>
        private byte[] LoadTile(int level, int row, int column)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT tile FROM '" + this.Url + "' WHERE level='" + level + "' AND row='" + row + "' AND column='" + column + "'";
            object result = cmd.ExecuteScalar();
            return result != null ? (byte[])result : null;
        }

        //判断表是否存在
        private bool IsTableExists(string name)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + name + "'";
            long i = (long)cmd.ExecuteScalar();
            return i == 0 ? false : true;
        }

        //图片流转二进制数组
        private byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private string ConvertObjectToJsonString(object objectToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
                serializer.WriteObject(ms, objectToSerialize);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private T ConvertJsonStringToObject<T>(string stringToDeserialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes = UnicodeEncoding.Unicode.GetBytes(stringToDeserialize);
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }


        //public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        //{
        //    BitmapImage bmp = null;
        //    try
        //    {
        //        bmp = new BitmapImage();
        //        bmp.BeginInit();
        //        bmp.StreamSource = new MemoryStream(byteArray);
        //        bmp.EndInit();
        //    }
        //    catch
        //    {
        //        bmp = null;
        //    }
        //    return bmp;
        //}

        /// <summary>
        /// LPY 该方法用来获取瓦片图到数据库
        /// </summary>
        private void getImages()
        {
            double bound;
            double row2;
            string url;
            HttpWebRequest req = null;
            Stream stream = null;

            int _level = 14;
            int _row = 1966;//983;//1966
            int _col = 3879;//1940;//3879

            int r_min = 0;
            int r_max = 0;
            int c_min = 0;
            int c_max = 0;

            //for (int l = 15; l < 16; l++)
            //{
            //    for (int r = 1966*2 - 2*2; r < 1966*2 + 2*2; r++)
            //    {
            //        for (int c = 3879*2 - 2*2; c < 3879*2 + 2*2; c++)
            //        {
            //            bound = Math.Pow(2, l);
            //            row2 = (bound * 0.17578125 - r - 1);
            //            url = _url + "Zoom=" + l + "&Row=" + row2 + "&Col=" + c + "";
            //            req = (HttpWebRequest)WebRequest.Create(url);
            //            stream = req.GetResponse().GetResponseStream();
            //            SaveTile(l, r, c, StreamToBytes(stream));
            //        }
            //    }
            //}

            for (int l = 14; l < 21; l++)
            {
                r_min = _row * (int)Math.Pow(2, l - _level) - (int)Math.Pow(2, l - _level + 1) - 10;
                r_max = _row * (int)Math.Pow(2, l - _level) + (int)Math.Pow(2, l - _level + 1) + 10;
                for (int r = r_min; r < r_max; r++)
                {
                    c_min = _col * (int)Math.Pow(2, l - _level) - (int)Math.Pow(2, l - _level + 1) - 10;
                    c_max = _col * (int)Math.Pow(2, l - _level) + (int)Math.Pow(2, l - _level + 1) + 10;
                    for (int c = c_min; c < c_max; c++)
                    {
                        bound = Math.Pow(2, l);
                        row2 = (bound * 0.17578125 - r - 1);
                        url = _url + "Zoom=" + l + "&Row=" + row2 + "&Col=" + c + "";
                        req = (HttpWebRequest)WebRequest.Create(url);
                        stream = req.GetResponse().GetResponseStream();
                        SaveTile(l, r, c, StreamToBytes(stream));
                    }
                }

            }

            //for (int l = 14; l < 21; l++)
            //{
            //    for (int r = (_row * (2 ^ (l - 14)) + 1 - (2 ^ (l - 14))); r < (_row * (2 ^ (l - 14)) + 1 + (2 ^ (l - 14))); r++)
            //    {
            //        for (int c = (_col * (2 ^ (l - 14)) + 1 - (2 ^ (l - 14))); c < (_col * (2 ^ (l - 14)) + 1 + (2 ^ (l - 14))); c++)
            //        {
            //            //GetTileUrl(_level, r, c);
            //            bound = Math.Pow(2, l);
            //            row2 = (bound * 0.17578125 - r - 1);
            //            url = _url + "Zoom=" + l + "&Row=" + row2 + "&Col=" + c + "";
            //            req = (HttpWebRequest)WebRequest.Create(url);
            //            stream = req.GetResponse().GetResponseStream();
            //            SaveTile(l, r, c, StreamToBytes(stream));
            //        }
            //    }
            //}

        }

    }

    class OneTile
    {
        public int level;
        public int row;
        public int col;
        public Stream e;

        public OneTile(int _level, int _row, int _col, Stream _e)
        {
            level = _level;
            row = _row;
            col = _col;
            e = _e;
        }

        public OneTile()
        {
        }

        //private byte[] StreamToBytes(Stream input)
        //{
        //    byte[] buffer = new byte[16 * 1024];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        int read;
        //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            ms.Write(buffer, 0, read);
        //        }
        //        return ms.ToArray();
        //    }
        //}

    }
}
