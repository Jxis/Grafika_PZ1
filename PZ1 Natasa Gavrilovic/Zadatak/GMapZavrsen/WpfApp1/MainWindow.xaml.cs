using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using WpfApp1.Common;
using WpfApp1.Model;
using Brushes = System.Drawing.Brushes;
using Pen = System.Drawing.Pen;
using Point = WpfApp1.Model.Point;
using Size = System.Drawing.Size;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //prvi deo
        public double noviX, noviY;
        public List<double> sviX = new List<double>();
        public List<double> sviY = new List<double>();
        public List<PowerEntity> listaSvihElementata = new List<PowerEntity>();
        public double najmanjiX, najveciX, najmanjiY, najveciY;
        public double xRastojanje, yRastojanje;

        public List<SwitchEntity> svitchevi = new List<SwitchEntity>();
        //public List<SwitchEntity> noviSvitcevi = new List<SwitchEntity>();

        public List<NodeEntity> nodovi = new List<NodeEntity>();
        //public List<NodeEntity> noviNodovi = new List<NodeEntity>();

        public List<SubstationEntity> substations = new List<SubstationEntity>();
        //public List<SubstationEntity> noviSubstations = new List<SubstationEntity>();

        public List<LineEntity> listaVodova = new List<LineEntity>();

        public static List<Point> points = new List<Point>();

        public List<Shape> markers = new List<Shape>();
        public List<Line> sveLinije = new List<Line>();

        public System.Windows.Media.Brush oldBrush = null;

        public double firstX, lastX, firstY, lastY;
        public double x, y;

        // morala sam napraviti zbog vodova jer sam skalirala koordinate sad 
        public List<PowerEntity> noviPowerEntity = new List<PowerEntity>();

        // Ideja: matrica boolova koji ce mi pokazivati da li se nesto nalazi vec na toj poziciji ili ne
        public bool[,] matrica = new bool[120, 200];

        // drugi deo
        private string selektovano = string.Empty;
        Nullable<bool> dialogResult;
        private System.Windows.Shapes.Polygon poligon = new System.Windows.Shapes.Polygon();
        private List<UIElement> ObrisaniElementi = new List<UIElement>();
        private Dictionary<int, List<UIElement>> recnikElemenataKojiSuClearovani = null;
        private int brojacKljuc = 0;

        public MainWindow()
        {
            InitializeComponent();
            recnikElemenataKojiSuClearovani = new Dictionary<int, List<UIElement>>();

        }


        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // ucitavam nodove,switcheve,substations i vodove
            ReadFromXML();

            //pravljenje grida
            //PravljenjeGrida();

            firstX = listaSvihElementata.Min(xx => xx.X);
            firstY = listaSvihElementata.Min(yy => yy.Y);

            lastX = listaSvihElementata.Max(xx => xx.X);
            lastY = listaSvihElementata.Max(yy => yy.Y);

            SkaliranjeMatriceICanvasa();

            DodajVodove();

        }

        // ucitavam node switch i sub u listu PowerEntity i vodova u listu vodova 
        public void ReadFromXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");
            XmlNodeList nodeList;

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {

                SubstationEntity sub = new SubstationEntity();
                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                sub.ToolTip = "Substation: \nID: " + sub.Id + "\nNAME: " + sub.Name;
                ToLatLon(sub.X, sub.Y, 34, out noviX, out noviY); // bilo u zadatku 
                sviX.Add(noviX);
                sviY.Add(noviY);

                sub.X = noviX;
                sub.Y = noviY;

                listaSvihElementata.Add(sub);
                substations.Add(sub); // dodat odmah iz xml 
            }


            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {

                NodeEntity nodeobj = new NodeEntity();
                nodeobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeobj.Name = node.SelectSingleNode("Name").InnerText;
                nodeobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nodeobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                nodeobj.ToolTip = "Node: \nID: " + nodeobj.Id + "\nNAME: " + nodeobj.Name;
                listaSvihElementata.Add(nodeobj);
                nodovi.Add(nodeobj);


                ToLatLon(nodeobj.X, nodeobj.Y, 34, out noviX, out noviY);
                sviX.Add(noviX);
                sviY.Add(noviY);

                nodeobj.X = noviX;
                nodeobj.Y = noviY;

            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchobj = new SwitchEntity();
                switchobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchobj.Name = node.SelectSingleNode("Name").InnerText;
                switchobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                switchobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                switchobj.Status = node.SelectSingleNode("Status").InnerText;

                switchobj.ToolTip = "Switch: \nID: " + switchobj.Id + "\nNAME: " + switchobj.Name;
                listaSvihElementata.Add(switchobj);
                svitchevi.Add(switchobj);


                ToLatLon(switchobj.X, switchobj.Y, 34, out noviX, out noviY);
                sviX.Add(noviX);
                sviY.Add(noviY);

                switchobj.X = noviX;
                switchobj.Y = noviY;

            }

            // ucitavanje vodova 
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in nodeList)
            {
                LineEntity l = new LineEntity();
                l.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                l.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    l.IsUnderground = true;
                }
                else
                {
                    l.IsUnderground = false;
                }
                l.R = float.Parse(node.SelectSingleNode("R").InnerText);
                l.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                l.LineType = node.SelectSingleNode("LineType").InnerText;
                l.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                l.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                l.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);


                foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes) // 9 posto je Vertices 9. node u jednom line objektu
                {
                    Point p = new Point();

                    p.X = double.Parse(pointNode.SelectSingleNode("X").InnerText);
                    p.Y = double.Parse(pointNode.SelectSingleNode("Y").InnerText);

                    ToLatLon(p.X, p.Y, 34, out noviX, out noviY);


                }

                listaVodova.Add(l);

            }
        }


        public List<Elipse> listaElipsi = new List<Elipse>();

        public void SkaliranjeMatriceICanvasa()
        {
            int newX;
            int newY;
            int counter = 0;

            List<Tuple<int, int>> zauzetaMesta = new List<Tuple<int, int>>();

            foreach (var el in listaSvihElementata)
            {

                newX = (int)((199 * (el.Y - firstY)) / (lastY - firstY));
                newY = (int)((119 * (el.X - firstX)) / (lastX - firstX));

                // logika za pronalazenje prazne pozicije u matrici
                int col = 0;
                int row = 0;
                int korak = 1;

                bool temp = false;


                while (temp == false)
                {

                    zauzetaMesta.Add(new Tuple<int, int>(newX + korak, newY + korak));
                    zauzetaMesta.Add(new Tuple<int, int>(newX + korak, newY - korak));
                    zauzetaMesta.Add(new Tuple<int, int>(newX - korak, newY + korak));
                    zauzetaMesta.Add(new Tuple<int, int>(newX - korak, newY - korak));

                    for (int i = korak - 1; i > -korak; i--)
                    {
                        zauzetaMesta.Add(new Tuple<int, int>(newX + korak, newY + i));
                        zauzetaMesta.Add(new Tuple<int, int>(newX + i, newY - korak));
                        zauzetaMesta.Add(new Tuple<int, int>(newX - korak, newY + i));
                        zauzetaMesta.Add(new Tuple<int, int>(newX + i, newY + korak));
                    }

                    foreach (var mesto in zauzetaMesta)
                    {
                        if (mesto.Item1 >= 0 && mesto.Item1 < 120 && mesto.Item2 >= 0 && mesto.Item2 < 200)
                        {
                            // ako nije zauzeto 
                            if (matrica[mesto.Item1, mesto.Item2] == false)
                            {
                                matrica[mesto.Item1, mesto.Item2] = true;

                                Ellipse elipsa = new Ellipse();
                                elipsa.Width = 3;
                                elipsa.Height = 3;
                                if (el.GetType() == typeof(SubstationEntity))
                                {
                                    elipsa.Fill = System.Windows.Media.Brushes.Red; // CRVENI SUBSTATIONS
                                    elipsa.ToolTip = "SUBSTATION: " + "\nNAME: " + el.Name + "\nID: " + el.Id;
                                }
                                else if (el.GetType() == typeof(NodeEntity))
                                {
                                    elipsa.Fill = System.Windows.Media.Brushes.Green; // ZELENI NODE
                                    elipsa.ToolTip = "NODE: " + "\nNAME: " + el.Name + "\nID: " + el.Id;
                                }
                                else if (el.GetType() == typeof(SwitchEntity))
                                {
                                    elipsa.Fill = System.Windows.Media.Brushes.Blue; // PLAVI SWITCH
                                    elipsa.ToolTip = "SWITCH: " + "\nNAME: " + el.Name + "\nID: " + el.Id;
                                }

                                counter++;
                                temp = true;

                                Canvas.SetLeft(elipsa, (newX + col) * 5); // 5 piksela
                                Canvas.SetBottom(elipsa, (newY + row) * 5);

                                PowerEntity p = el;
                                p.X = (newX + col) * 5;
                                p.Y = (newY + row) * 5;

                                noviPowerEntity.Add(p);

                                canvas.Children.Add(elipsa);

                                break;

                            }
                        }
                    }

                    korak++;

                }

                Console.WriteLine("Iscrtano objekata = " + counter);

                #region proba1
                //while(true)
                //{
                //    if(newX + col < 200 && newY + row < 120)
                //    {
                //        if (matrica[newY + row, newX + col] == false)
                //        {
                //            // nasao
                //            matrica[newY + row, newX + col] = true;

                //            Ellipse elipsa = new Ellipse();
                //            elipsa.Width = 3;
                //            elipsa.Height = 3;
                //            elipsa.Fill = System.Windows.Media.Brushes.Red;
                //            elipsa.ToolTip = el.Id.ToString();


                //            Canvas.SetLeft(elipsa, (newX + col) * 5); // 5 piksela
                //            Canvas.SetBottom(elipsa, (newY + row) * 5);

                //            canvas.Children.Add(elipsa);
                //            counter++;
                //            break;
                //        }
                //        else
                //        {
                //            // 
                //            // izmeni 
                //            col++;
                //            //row++;
                //            if (newX + col >= 200)
                //            {
                //                col = 0;
                //                row++;
                //            }
                //            if (newY + row >= 120)
                //            {
                //                break;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
                #endregion

            }
        }

        public Dictionary<long, long> vodoviKoordinate = new Dictionary<long, long>();
        public List<LineEntity> listaSvihLinijaVodovaKraj = new List<LineEntity>();
        public List<PowerEntity> pomocnaEntityZaMenjanjeBoje = new List<PowerEntity>();

        public void DodajVodove()
        {

            foreach (var vod in listaVodova)
            {
                double firstX = -1;
                double firstY = -1;
                double secondX = -1;
                double secondY = -1;

                foreach (var ent in noviPowerEntity)
                {
                    if (vod.FirstEnd == ent.Id)
                    {
                        firstX = ent.X;
                        firstY = ent.Y;
                    }
                    if (vod.SecondEnd == ent.Id)
                    {
                        secondX = ent.X;
                        secondY = ent.Y;
                    }
                }

                if (firstX == -1 || firstY == -1 || secondX == -1 || secondY == -1)
                {
                    continue;
                }

                // iscrtaj vodove sa polyline 
                Polyline mojaLinija = new Polyline();
                mojaLinija.Stroke = System.Windows.Media.Brushes.White;
                mojaLinija.StrokeThickness = 1;
                //mojaLinija.FillRule = FillRule.EvenOdd;
                // 1.5 jer mi je elipsa 3
                System.Windows.Point Point4 = new System.Windows.Point(firstX + 1.5, 600 - firstY - 1.5);
                System.Windows.Point Point5 = new System.Windows.Point(firstX + 1.5, 600 - secondY - 1.5);
                System.Windows.Point Point6 = new System.Windows.Point(secondX + 1.5, 600 - secondY - 1.5);

                PointCollection myPointCollection2 = new PointCollection();
                // sve jedno da li je prvo 6 ili 4
                myPointCollection2.Add(Point4);
                myPointCollection2.Add(Point5);
                myPointCollection2.Add(Point6);

                // kad se stisne na point da se promeni boja pointa koji ima koordinate firstx first y second x second y 

                LineEntity linija1 = new LineEntity();
                linija1.X1 = firstX + 1.5;
                linija1.Y1 = 600 - firstY - 1.5;
                linija1.X2 = firstX + 1.5;
                linija1.Y2 = 600 - secondY - 1.5;

                LineEntity linija2 = new LineEntity();
                linija2.X1 = firstX + 1.5;
                linija2.Y1 = 600 - secondY - 1.5;
                linija1.X2 = secondX + 1.5;
                linija1.Y2 = 600 - secondY - 1.5;

                // sadrzace listu svih nacrtanih linija 
                listaSvihLinijaVodovaKraj.Add(linija1);
                listaSvihLinijaVodovaKraj.Add(linija2);

                PowerEntity pe = new PowerEntity();
                pe.X = firstX;
                pe.Y = firstY;
                PowerEntity pe1 = new PowerEntity();
                pe1.X = secondX;
                pe1.Y = secondY;

                pomocnaEntityZaMenjanjeBoje.Add(pe);
                pomocnaEntityZaMenjanjeBoje.Add(pe1);


                mojaLinija.Points = myPointCollection2;

                //Panel.SetZIndex(mojaLinija, 2);
                canvas.Children.Add(mojaLinija);

            }
        }

        // Selectovana elipsa
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            selektovano = "elipsa";
            DugmeElipsa.BorderBrush = System.Windows.Media.Brushes.Red;
            DugmePolygon.BorderBrush = System.Windows.Media.Brushes.Black;
            DugmeText.BorderBrush = System.Windows.Media.Brushes.Black;

        }

        // Selectovan Polygon
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            poligon = new System.Windows.Shapes.Polygon();
            selektovano = "polygon";

            DugmePolygon.BorderBrush = System.Windows.Media.Brushes.Red;
            DugmeElipsa.BorderBrush = System.Windows.Media.Brushes.Black;
            DugmeText.BorderBrush = System.Windows.Media.Brushes.Black;

        }

        // Selectovan Text
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            selektovano = "text";
            DugmeElipsa.BorderBrush = System.Windows.Media.Brushes.Black;
            DugmePolygon.BorderBrush = System.Windows.Media.Brushes.Black;
            DugmeText.BorderBrush = System.Windows.Media.Brushes.Red;

        }

        // Generisanje Elipsa i Text
        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // obrisi prethodni kad udje opet 

            System.Windows.Point p = e.GetPosition(canvas);

            switch (selektovano)
            {
                case "elipsa":
                    // moja klasa 
                    Elipse prozor = new Elipse();
                    TextBlock block = new TextBlock();
                    dialogResult = prozor.ShowDialog();

                    if (dialogResult.Value)
                    {
                        System.Windows.Shapes.Ellipse elipsa = new Ellipse();

                        elipsa.Width = double.Parse(prozor.X.Text);
                        elipsa.Height = double.Parse(prozor.Y.Text);
                        elipsa.StrokeThickness = double.Parse(prozor.debljina.Text);
                        elipsa.Stroke = new SolidColorBrush(((Boja)prozor.ColorComboBox.SelectedItem).Bojica);
                        elipsa.Fill = new SolidColorBrush(((Boja)prozor.BojaComboBox.SelectedItem).Bojica);

                        elipsa.MouseLeftButtonDown += OnElipseMouseLeftButtonDown;

                        Canvas.SetTop(elipsa, p.Y);
                        Canvas.SetLeft(elipsa, p.X);
                        canvas.Children.Add(elipsa);

                        block.Text = prozor.textContenct.Text.ToString();
                        if (block.Text != "")
                        {
                            block.Foreground = new SolidColorBrush(((Boja)prozor.TextComboBox.SelectedItem).Bojica);
                            Canvas.SetTop(block, p.Y + elipsa.Width / 2);
                            Canvas.SetLeft(block, p.X + elipsa.Height / 2);
                            canvas.Children.Add(block);
                        }

                    }
                    break;
                case "polygon":
                    // samo dodaje tackice 
                    poligon.Points.Add(p);
                    break;
                case "text":
                    Text text = new Text();
                    dialogResult = text.ShowDialog();

                    if (dialogResult.Value)
                    {
                        TextBlock textBlock = new TextBlock();

                        textBlock.FontSize = double.Parse(text.velicina.Text);
                        textBlock.Text = text.sadrzaj.Text.ToString();
                        textBlock.Foreground = new SolidColorBrush(((Boja)text.BojaComboBox.SelectedItem).Bojica);

                        Canvas.SetLeft(textBlock, p.X);
                        Canvas.SetTop(textBlock, p.Y);
                        canvas.Children.Add(textBlock);

                    }
                    break;

                default:
                    break;

            }

            // ako stisnem na vod udje ovd 
            if (e.OriginalSource is Polyline)
            {
                Polyline poli = (Polyline)e.OriginalSource;

                double x1 = poli.Points[0].X - 1.5;
                double y1 = 600 - poli.Points[0].Y - 1.5;

                double x2 = poli.Points[2].X - 1.5;
                double y2 = 600 - poli.Points[2].Y - 1.5;

                Ellipse elipsa = new Ellipse();
                elipsa.Width = 3;
                elipsa.Height = 3;

                elipsa.Fill = System.Windows.Media.Brushes.Pink;
                elipsa.ToolTip = "ENTITET KOJI JE OZNACEN SA VODOM";

                Canvas.SetLeft(elipsa, x1); // 5 piksela
                Canvas.SetBottom(elipsa, y1);

                //Panel.SetZIndex(elipsa, 0);
                canvas.Children.Add(elipsa);



                Ellipse elipsa1 = new Ellipse();
                elipsa1.Width = 3;
                elipsa1.Height = 3;

                elipsa1.Fill = System.Windows.Media.Brushes.Pink;
                elipsa1.ToolTip = "ENTITET 1 KOJI JE OZNACEN SA VODOM";

                Canvas.SetLeft(elipsa1, x2);
                Canvas.SetBottom(elipsa1, y2);
                canvas.Children.Add(elipsa1);


            }

        }

        // Generisanje Polygon 
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (selektovano.Equals("polygon") && poligon != null)
            {
                // za poligon su potrebne 4 tacke, uslov prolazi kad je Count 3, a 4 treba da bude zapravo prva tacka a to sama klasa
                // Poligon dodaje
                if (poligon.Points.Count > 2)
                {
                    Polygon prozor = new Polygon();
                    TextBlock block = new TextBlock();
                    dialogResult = prozor.ShowDialog();



                    if (dialogResult.Value)
                    {
                        poligon.StrokeThickness = double.Parse(prozor.debljina.Text);
                        poligon.Stroke = new SolidColorBrush(((Boja)prozor.ColorComboBox.SelectedItem).Bojica);
                        poligon.Fill = new SolidColorBrush(((Boja)prozor.BojaComboBox.SelectedItem).Bojica);

                        block.Text = prozor.textContenct.Text.ToString();
                        //block.Foreground = new SolidColorBrush(((Boja)prozor.TextComboBox.SelectedItem).Bojica);

                        poligon.MouseLeftButtonDown += OnPolygonMouseLeftButtonDown;

                        // sam postavlja poligon na odgovarajucu poziciju 

                        canvas.Children.Add(poligon);
                        // ISPRAVI OVO DA JE U SREDINI 
                        if (block.Text != "")
                        {
                            block.Foreground = new SolidColorBrush(((Boja)prozor.TextComboBox.SelectedItem).Bojica);
                            canvas.Children.Add(block);
                        }

                        poligon = new System.Windows.Shapes.Polygon();

                    }
                }
            }


        }

        // Clear
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            selektovano = string.Empty;

            if (canvas.Children.Count > 0)
            {
                List<UIElement> temp = new List<UIElement>();
                foreach (UIElement element in canvas.Children)
                {
                    temp.Add(element);
                }
                recnikElemenataKojiSuClearovani.Add(brojacKljuc, temp);
                brojacKljuc++;
                canvas.Children.Clear();
            }

        }

        // Undo
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

            selektovano = string.Empty;

            if (canvas.Children.Count > 0)
            {
                ObrisaniElementi.Add(canvas.Children[canvas.Children.Count - 1]);
                canvas.Children.Remove(canvas.Children[canvas.Children.Count - 1]);
            }
            else if (canvas.Children.Count == 0 && recnikElemenataKojiSuClearovani.Count > 0)
            {

                if (recnikElemenataKojiSuClearovani.ContainsKey(--brojacKljuc))
                {
                    recnikElemenataKojiSuClearovani[brojacKljuc].ForEach(element => canvas.Children.Add(element));

                    recnikElemenataKojiSuClearovani.Remove(brojacKljuc);
                }
            }
        }

        // Redo
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

            selektovano = string.Empty;

            if (ObrisaniElementi.Count > 0)
            {
                canvas.Children.Add(ObrisaniElementi[ObrisaniElementi.Count - 1]);
                ObrisaniElementi.RemoveAt(ObrisaniElementi.Count - 1);
            }
        }


         
        void OnElipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!selektovano.Equals("polygon") || poligon.Points.Count <= 2)
            {
                System.Windows.Shapes.Ellipse el = (System.Windows.Shapes.Ellipse)e.OriginalSource;
                //TextBlock te = (TextBlock)e.OriginalSource;

                // ako imam samo elipsu 
                if (canvas.Children.Contains(el))
                {
                    int index = canvas.Children.IndexOf(el);

                    Elipse prozor = new Elipse();

                    //popuni sa vec postojecim atributima
                    prozor.X.Text = el.Width.ToString();
                    prozor.X.IsReadOnly = true;
                    prozor.Y.Text = el.Height.ToString();
                    prozor.Y.IsReadOnly = true;
                    prozor.debljina.Text = el.StrokeThickness.ToString();
                    prozor.debljina.IsReadOnly = true;
                    prozor.BojaComboBox.SelectedValue = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(el.Fill.ToString());
                    prozor.ColorComboBox.SelectedValue = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(el.Stroke.ToString());

                    dialogResult = prozor.ShowDialog();
                    //promeni atribute
                    if (dialogResult.Value)
                    {
                        // konturne linije i debljina 
                        el.Fill = new SolidColorBrush(((Boja)prozor.BojaComboBox.SelectedItem).Bojica);
                        el.Stroke = new SolidColorBrush(((Boja)prozor.ColorComboBox.SelectedItem).Bojica);

                        canvas.Children.RemoveAt(index);
                        canvas.Children.Insert(index, el);
                    }

                }
                

            }
        }

        
        void OnPolygonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!selektovano.Equals("polygon") || poligon.Points.Count <= 2)
            {
                System.Windows.Shapes.Polygon shape = (System.Windows.Shapes.Polygon)e.OriginalSource;
                //TextBlock te = (TextBlock)e.OriginalSource;

                if (canvas.Children.Contains(shape))
                {
                    int index = canvas.Children.IndexOf(shape);

                    Polygon prozor = new Polygon();
                    //popuni sa vec postojecim atributima
                    prozor.debljina.Text = shape.StrokeThickness.ToString();
                    prozor.debljina.IsReadOnly = true;
                    prozor.BojaComboBox.SelectedValue = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(shape.Fill.ToString());
                    prozor.ColorComboBox.SelectedValue = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(shape.Stroke.ToString());

                    dialogResult = prozor.ShowDialog();
                    //promeni atribute
                    if (dialogResult.Value)
                    {
                        // ne moze dami menja points jer ja klikom ih oznacavam 
                        shape.Fill = new SolidColorBrush(((Boja)prozor.BojaComboBox.SelectedItem).Bojica);
                        shape.Stroke = new SolidColorBrush(((Boja)prozor.ColorComboBox.SelectedItem).Bojica);

                        canvas.Children.RemoveAt(index);
                        canvas.Children.Insert(index, shape);
                    }
                }

            }
        }
    


        //From UTM to Latitude and longitude in decimal
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
		{
			bool isNorthHemisphere = true;

			var diflat = -0.00066286966871111111111111111111111111;
			var diflon = -0.0003868060578;

			var zone = zoneUTM;
			var c_sa = 6378137.000000;
			var c_sb = 6356752.314245;
			var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
			var e2cuadrada = Math.Pow(e2, 2);
			var c = Math.Pow(c_sa, 2) / c_sb;
			var x = utmX - 500000;
			var y = isNorthHemisphere ? utmY : utmY - 10000000;

			var s = ((zone * 6.0) - 183.0);
			var lat = y / (c_sa * 0.9996);
			var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
			var a = x / v;
			var a1 = Math.Sin(2 * lat);
			var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
			var j2 = lat + (a1 / 2.0);
			var j4 = ((3 * j2) + a2) / 4.0;
			var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
			var alfa = (3.0 / 4.0) * e2cuadrada;
			var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
			var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
			var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
			var b = (y - bm) / v;
			var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
			var eps = a * (1 - (epsi / 3.0));
			var nab = (b * (1 - epsi)) + lat;
			var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
			var delt = Math.Atan(senoheps / (Math.Cos(nab)));
			var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

			longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
			latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
		}
	}
}
