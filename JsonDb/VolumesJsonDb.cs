using System;
using System.Text;
using Crestron.SimplSharp;                         				// For Basic SIMPL# Classes
using Newtonsoft.Json;                                          //Thanks to Neil Colvin. Full Library @ http://www.nivloc.com/downloads/crestron/SSharp/
using Crestron.SimplSharp.CrestronIO;
using System.Collections.Generic;
using JsonDb;
//using SpmLibrary;

namespace JsonDb
{

    public class VolumesJsonDb
    {
        public VolumeConfig MyVolumeConfig;
        private string JsonString;
        //private JsonDatabase MyJsonDatabase;     

        // Public delegates
        public OutputVolValDel OutputVolVal { get; set; }
        public delegate void OutputVolValDel(ushort value, ushort volumeIndex);
        //public delegate void OutputCirCountDel(int roomIndex, int sceneIndex);

        /*Pass the FilePath from SIMPL+ then read in the file.
        Create the JSON Object and use the library to deserialize it into 
        our classes.
        */

        public void Reader(string FilePath)
        {
            //MyJsonDatabase = new JsonDatabase<SceneConfig>();
            if (File.Exists(FilePath))       //Ok make sure the file is there
            {
                StreamReader JsonFile = new StreamReader(FilePath);
                JsonString = JsonFile.ReadToEnd();
                JsonFile.Close();
                CrestronConsole.PrintLine("File found" + JsonString + "\n\r");    //Generate error
            }
            else
            {
                CrestronConsole.PrintLine("File Not found\n\r");    //Generate error
                JsonString = "";

            }

            CrestronConsole.PrintLine("Extractor Starting...\n\r");    //Generate error
            MyVolumeConfig = JsonConvert.DeserializeObject<VolumeConfig>(JsonString);
            CrestronConsole.PrintLine("Extractor Finished...\n\r");    //Generate error 
        }

        public void getVolumeValue(ushort presetIndex, ushort volumeIndex)
        {
            CrestronConsole.PrintLine("Get Volume Value preset={0} volume={1} value={2}\n\r", presetIndex, volumeIndex, MyVolumeConfig.Presets[presetIndex].Volumes[volumeIndex].Value);
            if (OutputVolVal != null)
                OutputVolVal(MyVolumeConfig.Presets[presetIndex].Volumes[volumeIndex].Value, volumeIndex);
        }

        public void setVolumeValue(ushort presetIndex, ushort VolumeIndex, ushort value, string FilePath)
        {
            CrestronConsole.PrintLine("Set Volume Value preset={0} volume={1} value={2}\n\r", presetIndex, VolumeIndex, value);
            MyVolumeConfig.Presets[presetIndex].Volumes[VolumeIndex].Value = value;

            StreamWriter file = new StreamWriter(FilePath);
            string ConfigSave = JsonConvert.SerializeObject(MyVolumeConfig);
            file.Write(ConfigSave);
            file.Flush();
            file.Close();
        }

        public class VolumeConfig
        {
            [JsonProperty("Presets")]
            public IList<Preset> Presets { get; set; }
        }

        public class Preset
        {
            [JsonProperty("PresetID")]
            public ushort PresetID { get; set; }
            [JsonProperty("Volumes")]
            public IList<Volume> Volumes { get; set; }
        }

        public class Volume
        {
            [JsonProperty("VolumeID")]
            public ushort VolumeID { get; set; }
            [JsonProperty("Value")]
            public ushort Value { get; set; }
        }
    }
}