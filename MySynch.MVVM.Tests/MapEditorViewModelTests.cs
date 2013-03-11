using System.IO;
using MySynch.Monitor.MVVM.ViewModels;
using NUnit.Framework;

namespace MySynch.MVVM.Tests
{
    [TestFixture]
    public class MapEditorViewModelTests
    {
        [Test]
        public void LoadTheMap_Ok()
        {
            MapEditorViewModel mapEditorViewModel= new MapEditorViewModel();
            Assert.IsNotNull(mapEditorViewModel.MapChannels);
            Assert.AreEqual(3,mapEditorViewModel.MapChannels.Count);

        }

        [Test]
        public void LoadTheMap_FileNotFound()
        {
            if (File.Exists(@"Data\map\distributormap.xml"))
            {
                File.Copy(@"Data\map\distributormap.xml",@"Data\map\distributormap1.xml",true);
                File.Delete(@"Data\map\distributormap.xml");
            }
            MapEditorViewModel mapEditorViewModel = new MapEditorViewModel();
            Assert.IsNull(mapEditorViewModel.MapChannels);
            if (File.Exists(@"Data\map\distributormap1.xml"))
            {
                File.Copy(@"Data\map\distributormap1.xml", @"Data\map\distributormap.xml", true);
                File.Delete(@"Data\map\distributormap1.xml");
            }

        }

        [Test]
        public void LoadTheMap_NothingInFile()
        {
            if (File.Exists(@"Data\map\distributormap.xml"))
            {
                File.Copy(@"Data\map\distributormap.xml", @"Data\map\distributormap1.xml", true);
                File.Copy(@"Data\map\distributormap2.xml",@"Data\map\distributormap.xml",true);
            }
            MapEditorViewModel mapEditorViewModel = new MapEditorViewModel();
            Assert.IsEmpty(mapEditorViewModel.MapChannels);
            if (File.Exists(@"Data\map\distributormap1.xml"))
            {
                File.Copy(@"Data\map\distributormap.xml", @"Data\map\distributormap1.xml", true);
                File.Delete(@"Data\map\distributormap1.xml");
            }

        }
    }
}
