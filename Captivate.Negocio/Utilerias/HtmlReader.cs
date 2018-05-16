﻿using Captivate.Comun.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio.Utilerias
{
    public class HtmlFileReader : IFileReader
    {
        public string GetContent(string path)
        {
            //Lee el html
            string htmlString = System.IO.File.ReadAllText(path);
            return htmlString;
        }
    }
}