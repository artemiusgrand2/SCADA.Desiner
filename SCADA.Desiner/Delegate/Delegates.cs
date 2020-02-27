using System;
using System.Collections.Generic;
using SCADA.Desiner.Enums;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.Inteface;
using SCADA.Common.SaveElement;
using SCADA.Common.Enums;

namespace SCADA.Desiner.Delegate
{
    public delegate void Delete();
    public delegate void Rotate(double angle);
    public delegate void Reverse();
    // public delegate void NewColor(byte R, byte G, byte B);
    public delegate void NewColor(NameColors NameColor);
    public delegate void NewWeight(double Weight);
    public delegate void IsFillInside();
    public delegate void NewScroll(double Scroll);
    public delegate void NewSize(double width, double height);
    public delegate void NewFamilyFont(string FamilyFont);
    public delegate void addelement();
    public delegate void visiblesetka(bool Checked);
    public delegate void SpisokId(List<int> Id, PointFigure point);
    public delegate void updateelement(List<MyTable> tables);
    public delegate void updateelementCommand(string name, string help, ViewCommand viewcommand, ViewPanel viewpanel, int id);

    public delegate void NewTaktEvent();
    public delegate void Sizedelta(double deltax, double deltay);
    public delegate void AddObject(List<IGraficObejct> objects);
    public delegate void RemoveObject(List<IGraficObejct> objects);
    public delegate void UpdateObject(List<IGraficObejct> objects);
    public delegate void UpdateGeometry(List<IGraficObejct> objects);
    public delegate void SelectObject(List<IGraficObejct> objects);
    public delegate void UpdateCountTime(int cout);
    public delegate void NewSourceForArea(string path, ViewArea viewArea);
    public delegate void NewLayer(ViewLayer viewlayer, int layer);
    public delegate void HatchLine(IList<double> arrayhatch);
}
