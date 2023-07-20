using GalE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalE.Script
{
    public abstract class ScriptBase
    {
        public virtual void Init(GEngin engin)
        {}

        public virtual void Update(GEngin engin)
        {}
    }
}