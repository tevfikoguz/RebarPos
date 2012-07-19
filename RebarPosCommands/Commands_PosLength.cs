﻿using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

using OZOZ.RebarPosWrapper;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void ShowPosLength(IEnumerable<ObjectId> list, bool show)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in list)
                    {
                        RebarPos pos = tr.GetObject(id, OpenMode.ForWrite) as RebarPos;
                        if (pos != null)
                        {
                            if (pos.Display == RebarPos.DisplayStyle.MarkerOnly) continue;
                            if (show)
                                pos.Display = RebarPos.DisplayStyle.All;
                            else
                                pos.Display = RebarPos.DisplayStyle.WithoutLength;
                        }
                    }

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
