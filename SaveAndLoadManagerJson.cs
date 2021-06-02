using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class SaveAndLoadManagerJson<T> where T : class //typ generyczny T
    {
        public void Save(T objectToSave)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "gol files (*.gol)|*.gol";
            if (saveFileDialog.ShowDialog() == true)
            {
                string serializedJson = JsonConvert.SerializeObject(objectToSave);
                File.Delete(saveFileDialog.FileName);
                File.AppendAllText(saveFileDialog.FileName, serializedJson);
            }
        }


        public T Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "gol files (*.gol)|*.gol";
            if (openFileDialog.ShowDialog() == true)
            {
                var serializedJson = File.ReadAllText(openFileDialog.FileName);
                T objectToLoad = JsonConvert.DeserializeObject<T>(serializedJson);  //typem objectToLoad jest T (typ przyjmowany w danej chwili,zdefiniowany)
                return objectToLoad;
            }

            return null; 
        }
    }
}
