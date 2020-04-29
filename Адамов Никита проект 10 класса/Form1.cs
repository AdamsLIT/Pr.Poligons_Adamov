using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Shapes;
using Rectangle = Shapes.Rectangle;
using System.Reflection;

namespace Адамов_Никита_проект_10_класса
{
    public partial class Form1 : Form
    {
        Type ShapeType;
        int Convex_hull_Type;
        int TicsInterval;
        List<IAction> action;
        Color ColorInside;
        RadiusChange form2;
        TimerValue form3;
        string FileName;
        bool save;
        int x, y;
        Stack<List<IAction>> UnDo_Stack;
        Stack<List<IAction>> ReDo_Stack;
        List<Shape> shapes;
        List<Assembly> assemblies;
        bool ToMoveSmth;
        Random rnd = new Random();
        private System.Timers.Timer aTimer;
        public Form1()
        {
            InitializeComponent();
            circleToolStripMenuItem.Tag = typeof(Circle);
            rectangleToolStripMenuItem.Tag = typeof(Rectangle);
            triangleToolStripMenuItem.Tag = typeof(Trinagle);
            assemblies = new List<Assembly>();
            this.Text = "Файл не сохранен";
            FileName = null;
            save = true;
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "will always be first|*.cska";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.InitialDirectory = "C:";
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "will always be first|*.cska";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.InitialDirectory = "C:";
            ColorInside = Color.Yellow;
            shapes = new List<Shape>();
            ShapeType = typeof(Circle);
            Convex_hull_Type = 1;
            ToMoveSmth = false;
            action = new List<IAction>();
            TicsInterval = 500;
            aTimer = new System.Timers.Timer(TicsInterval);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Stop();
            UnDo_Stack = new Stack<List<IAction>>();
            ReDo_Stack = new Stack<List<IAction>>();
            Plugin_Load();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            //MessageBox.Show(typeof(Shape).ToString());
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                foreach (Assembly assembly in assemblies)
                {
                    if (args.Name == assembly.FullName)
                        return assembly;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
       
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            if (shapes.Count > 2)
            {
                if (Convex_hull_Type == 1)
                    Convex_hull_by_Dzarvis(e.Graphics);
                else if (Convex_hull_Type == 2)
                    Convex_hull_by_Definition(e.Graphics);///////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////
                
            }

            foreach (Shape Object in shapes)
            {
                Object.Draw(e.Graphics);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool Inside = false;

            if (e.Button == MouseButtons.Left)
            {
                foreach (Shape Object in shapes)
                {
                    if (Object.Inside(e.X, e.Y))
                    {
                        x = e.X;
                        y = e.Y;
                        Inside = true;
                        Object.MouseX1 = e.X - Object.X1;
                        Object.MouseY1 = e.Y - Object.Y1;
                        Object.ToMove1 = true;
                        ToMoveSmth = true;
                    }
                }
                if (Inside) return;
                if (shapes.Count > 2 && Inside_Of_Convex_hull(e))
                {
                    foreach (Shape Object in shapes)
                    {
                        ToMoveSmth = true;
                        Object.MouseX1 = e.X - Object.X1;
                        x = e.X;
                        Object.MouseY1 = e.Y - Object.Y1;
                        y = e.Y;
                        Object.ToMove1 = true;
                    }
                    return;
                }

                ToMoveSmth = true;
                 ConstructorInfo f =  ShapeType.GetConstructor(new Type[] { typeof(int), typeof(int) });
                 Shape s = (Shape)f.Invoke(new object[] { e.X, e.Y });
                s.ToMove1 = true;
                save = false;
                shapes.Add(s);

                
               
                    UnDo_Stack.Push(new List<IAction>() { new AddShape(shapes[shapes.Count - 1], shapes.Count - 1) });
                    button3.Enabled = true;
                    ReDo_Stack.Clear();
                    button4.Enabled = false;
                
            }
            if (e.Button == MouseButtons.Right)
            {
                for (int i = shapes.Count - 1; i >= 0; i--)
                {
                    if (shapes[i].Inside(e.X, e.Y))
                    {
                        UnDo_Stack.Push(new List<IAction>() { new DeleteShape(shapes[i], i) });
                        button3.Enabled = true;
                        ReDo_Stack.Clear();
                        button4.Enabled = false;
                        shapes.RemoveAt(i);
                        save = false;
                        break;
                    }
                }
            }
            Refresh();
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            List<IAction> Peredviz = new List<IAction>();
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i].ToMove1 == true)
                {
                    if (UnDo_Stack.Peek()[0] is AddShape)
                    {
                        UnDo_Stack.Peek().Insert(0, new Moving(e.X - x, e.Y - y, i));
                    }
                    else
                    {
                        Peredviz.Add(new Moving(e.X - x, e.Y - y, i));
                    }

                    shapes[i].ToMove1 = false;
                }

            }
            if (Peredviz.Count != 0)
            {
                UnDo_Stack.Push(Peredviz);
                ReDo_Stack.Clear();
                button4.Enabled = false;
            }
            // Convex_hull.Delete_Points(shapes);
            if (shapes.Count > 2)
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].Extra_point == false)
                    {
                        UnDo_Stack.Peek().Insert(0, new DeleteShape(shapes[i], i));
                        shapes.RemoveAt(i);
                        i--;
                        ReDo_Stack.Clear();
                        button4.Enabled = false;
                    }
                }
            ToMoveSmth = false;
            Refresh();
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ToMoveSmth) return;
            foreach (Shape Object in shapes)
            {
                if (Object.ToMove1)
                {
                    Object.X1 = e.X - Object.MouseX1;
                    Object.Y1 = e.Y - Object.MouseY1;
                    button4.Enabled = false;//////////////////////
                    ReDo_Stack.Clear();///////////////////////////////////////
                }
            }

            Refresh();
        }



        private void shapeToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            if (ShapeType != (Type)e.ClickedItem.Tag)
            {
                UnDo_Stack.Push(new List<IAction>() { new Figure_Choosen(ShapeType, (Type)e.ClickedItem.Tag) });
                ShapeType = (Type)e.ClickedItem.Tag;
                save = false;
                button4.Enabled = false;//////////////////////
                ReDo_Stack.Clear();///////////////////////////////////////
            }
        }



        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }


        private void Convex_hull_by_Dzarvis(Graphics graphics)
        {
            List<Point> Tops = new List<Point>();
            Convex_hull obj = new Convex_hull();
            if (shapes.Count <= 2) return;
            Shape Start = obj.StartPoint(shapes);
            Point Current_Point = new Point(Start.X1, Start.Y1);
            Point Previous_Point = new Point(Current_Point.X + 1, Current_Point.Y);
            Shape Next_Point = null;
            double angle, Alpha;
            foreach (Shape figure in shapes)
            {
                figure.Extra_point = false;
            }
            Start.Extra_point = true;
            while (true)
            {
                angle = -7;
                foreach (Shape shape in shapes)
                {
                    Alpha = obj.Angle(Previous_Point, Current_Point, new Point(shape.X1, shape.Y1));
                    if (angle == Alpha)
                    {
                        if (Next_Point == null || obj.Dist(Current_Point, new Point(shape.X1, shape.Y1)) > obj.Dist(Current_Point, new Point(Next_Point.X1, Next_Point.Y1)))
                        {
                            Next_Point = shape;
                        }
                    }
                    if (angle < Alpha)
                    {
                        angle = Alpha;
                        Next_Point = shape;
                    }
                }
                if ((Next_Point.X1 == Start.X1 && Next_Point.Y1 == Start.Y1))
                {
                    break;
                }
                else
                {
                    Next_Point.Extra_point = true;
                    graphics.FillPolygon(new SolidBrush(ColorInside), new PointF[] { new PointF(Next_Point.X1, Next_Point.Y1), new PointF(Current_Point.X, Current_Point.Y), new PointF(shapes[0].X1, shapes[0].Y1) });
                }
                Previous_Point = new Point(Current_Point.X, Current_Point.Y);
                Current_Point = new Point(Next_Point.X1, Next_Point.Y1);

            }
            graphics.FillPolygon(new SolidBrush(ColorInside), new PointF[] { new PointF(Start.X1, Start.Y1), new PointF(Current_Point.X, Current_Point.Y), new PointF(shapes[0].X1, shapes[0].Y1) });
        }

        private void Convex_hull_by_Definition(Graphics graphics)
        {
            float A, B, C;
            bool Points_Left = false, Points_Right = false;
            foreach (Shape figure in shapes)
            {
                figure.Extra_point = false;
            }
            for (int i = 0; i < shapes.Count - 1; i++)
            {
                for (int j = i + 1; j < shapes.Count; j++)
                {
                    A = shapes[i].Y1 - shapes[j].Y1;
                    B = shapes[j].X1 - shapes[i].X1;
                    C = shapes[i].X1 * shapes[j].Y1 - shapes[j].X1 * shapes[i].Y1;
                    Points_Left = false; Points_Right = false;
                    foreach (Shape figure in shapes)
                    {
                        if (A * figure.X1 + B * figure.Y1 + C > 0)
                            Points_Left = true;
                        if (A * figure.X1 + B * figure.Y1 + C < 0)
                            Points_Right = true;
                    }

                    if (Points_Left != Points_Right)
                    {

                        graphics.FillPolygon(new SolidBrush(ColorInside), new PointF[] { new PointF(shapes[i].X1, shapes[i].Y1), new PointF(shapes[j].X1, shapes[j].Y1), new PointF(shapes[0].X1, shapes[0].Y1) });
                        shapes[i].Extra_point = true; shapes[j].Extra_point = true;

                    }

                }
            }

        }

        private void shapeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (aTimer.Enabled == true)
            {
                aTimer.Stop();
                save = false;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (aTimer.Enabled == false)
            {
                aTimer.Start();
                save = false;
            }
        }


        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            aTimer.Interval = TicsInterval;
            foreach (Shape figura in shapes)
            {
                figura.X1 += rnd.Next(-1, 2);
                figura.Y1 += rnd.Next(-1, 2);
            }
            this.Invoke(new Action(() => { Refresh(); }));
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {

            /*form2 = new RadiusChange(Shape.Radius);
            form2.OnRadiuschanged += Form2_OnRadiuschanged;
            form2.Show();*/

        }

        private void Form2_OnRadiuschanged(object sender, RadiusEventArgs e)
        {
            if (Shape.Radius != e.radius)
            {
                Shape.Radius = e.radius;
                save = false;
                
                Refresh();
            }


        }

        private void timerTicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*form3 = new TimerValue(TicsInterval);
            form3.OnTimerChanged += Form3_OnTimerChanged;
            form3.Show();*/

        }

        private void Form3_OnTimerChanged(object sender, TimerEventArgs e)
        {
            TicsInterval = e.TimerTics;
            save = false;

            Refresh();
        }

        private void schearchForToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void schearchForToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Dzarvis":
                    Convex_hull_Type = 1;
                    save = false;
                    break;
                case "Defenition":
                    Convex_hull_Type = 2;
                    save = false;
                    break;
                case "Radius":
                    {

                        if (form2 == null || form2.Created == false)
                        {
                            form2 = new RadiusChange(Shape.Radius);
                            form2.OnRadiuschanged += Form2_OnRadiuschanged;
                            form2.OnRadiudEndChanging += Form2_OnRadiudEndChanging;
                            form2.Show();

                        }
                        else
                        {
                            form2.Show();
                            form2.WindowState = FormWindowState.Maximized;

                        }
                    }
                    break;
                case "TimerTics":
                    {
                        if (form3 == null)
                        {
                            form3 = new TimerValue(TicsInterval);
                            form3.OnTimerChanged += Form3_OnTimerChanged;
                            form3.Show();
                        }
                        else
                        {
                            form3.Show();
                            form3.WindowState = FormWindowState.Maximized;
                        }

                    }
                    break;

            }
        }

        private void Form2_OnRadiudEndChanging(object sender, RadiusChangingEventArgs e)
        {
            UnDo_Stack.Push(new List<IAction>() { new Radius_changes(-e.radius_Old + e.radius_New) });
            button4.Enabled = false;//////////////////////
            ReDo_Stack.Clear();///////////////////////////////////////
            Refresh();

        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Convex_hull_Type = 1;
            save = false;
        }

        private void definitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Convex_hull_Type = 2;
            save = false;
        }

        private void colorOfLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                if (Shape.Color_of_figure != colorDialog1.Color)
                {
                    UnDo_Stack.Push(new List<IAction>() { new Color_changes_figures(colorDialog1.Color, Shape.Color_of_figure) });
                    button4.Enabled = false;//////////////////////
                    ReDo_Stack.Clear();///////////////////////////////////////
                }
                Shape.Color_of_figure = colorDialog1.Color;
                Refresh();
                save = false;
            }

        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool toclear = true;
            if (save == false)
            {
                switch (MessageBox.Show("Файл не был сохранен. Сохранить его?", "Файл не сохранен", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.No: toclear = false; break;
                    case DialogResult.Cancel: return;
                }
                if (FileName == null && toclear == true)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Serialized(File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate));
                        FileName = saveFileDialog1.FileName;

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (toclear == true)
                        Serialized(File.Open(FileName, FileMode.OpenOrCreate));
                }

            }
            shapes = new List<Shape>();
            save = true;
            FileName = null;
            Refresh();

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool toclear = true;
            if (save == false)
            {
                switch (MessageBox.Show("Файл не был сохранен. Сохранить его?", "Файл не сохранен", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.No: toclear = false; break;
                    case DialogResult.Cancel: return;
                }

                if (FileName == null && toclear == true)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Serialized(File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate));
                        FileName = saveFileDialog1.FileName;
                        this.Text = FileName.Substring(FileName.LastIndexOf("\\") + 1, FileName.Length - FileName.LastIndexOf("\\") - 6);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (toclear == true)
                        Serialized(File.Open(FileName, FileMode.OpenOrCreate));

                }
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != FileName)
                {
                    DeSerialized(File.Open(openFileDialog1.FileName, FileMode.OpenOrCreate));
                    save = true;
                    FileName = openFileDialog1.FileName;
                    this.Text = FileName.Substring(FileName.LastIndexOf("\\") + 1, FileName.Length - FileName.LastIndexOf("\\") - 6);
                    Refresh();
                }
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (save == false)
            {
                if (FileName == null)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Serialized(File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate));
                        save = true;
                        FileName = saveFileDialog1.FileName;
                        this.Text = FileName.Substring(FileName.LastIndexOf("\\") + 1, FileName.Length - FileName.LastIndexOf("\\") - 6);
                    }

                }
                else
                {
                    Serialized(File.Open(FileName, FileMode.OpenOrCreate));
                }
                Refresh();
            }

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != FileName)
                {
                    Serialized(File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate));
                    save = true;
                    FileName = saveFileDialog1.FileName;
                    this.Text = FileName.Substring(FileName.LastIndexOf("\\") + 1, FileName.Length - FileName.LastIndexOf("\\") - 6);
                    Refresh();
                }
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (save == false)
            {
                switch (MessageBox.Show("Файл не был сохранен. Сохранить его?", "Файл не сохранен", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.No: return;
                    case DialogResult.Cancel: e.Cancel = true; return;
                }
                if (FileName == null)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Serialized(File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate));
                        FileName = saveFileDialog1.FileName;

                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    Serialized(File.Open(FileName, FileMode.OpenOrCreate));
                }

            }
        }

        private void colorInsideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                if (ColorInside != colorDialog1.Color)
                {
                    UnDo_Stack.Push(new List<IAction>() { new Color_changes_hull(colorDialog1.Color, ColorInside) });
                    button4.Enabled = false;//////////////////////
                    ReDo_Stack.Clear();///////////////////////////////////////
                }
                ColorInside = colorDialog1.Color;
                Refresh();
                save = false;
            }

        }

        private void dzarvisToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }


        private bool Inside_Of_Convex_hull(MouseEventArgs e)
        {
            foreach (Shape Top1 in shapes)
            {
                foreach (Shape Top2 in shapes)
                {
                    if (new Create_Triangle(new PointD(Top1.X1, Top1.Y1), new PointD(Top2.X1, Top2.Y1), new PointD(shapes[0].X1, shapes[0].Y1)).Inside_of_Triangle(e.X, e.Y))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        public void AddShape(Shape To_add, int index)
        {
            shapes.Insert(index, To_add);

        }
        public void DeleteShape(int index)
        {
            shapes.RemoveAt(index);

        }

        private void button3_Click(object sender, EventArgs e)//Undo
        {
            foreach (IAction action in UnDo_Stack.Peek())
            {
                action.UnDo(this);
            }
            UnDo_Stack.Peek().Reverse();
            ReDo_Stack.Push(UnDo_Stack.Peek());

            button4.Enabled = true;
            UnDo_Stack.Pop();
            if (UnDo_Stack.Count == 0)
                button3.Enabled = false;
            Refresh();
        }

        private void button4_Click(object sender, EventArgs e)//Redo
        {
            foreach (IAction action in ReDo_Stack.Peek())
            {

                action.ReDo(this);
            }
            ReDo_Stack.Peek().Reverse();
            UnDo_Stack.Push(ReDo_Stack.Peek());
            button3.Enabled = true;
            ReDo_Stack.Pop();
            if (ReDo_Stack.Count == 0)
                button4.Enabled = false;
            Refresh();
        }

        public void Add_XY_to_Shape(int x, int y, int index)
        {
            shapes[index].X1 += x;
            shapes[index].Y1 += y;

        }
        public void Add_to_Radius(int delta)
        {
            Shape.Radius += delta;
            form2.Rad_Value += delta;

        }
        public void Return_Color_hull(Color color)
        {
            ColorInside = color;

        }
        public void Return_Color_figure(Color color)
        {
            Shape.Color_of_figure = color;

        }
        public void Return_Type_figure(Type ShapeType)
        {


            this.ShapeType = ShapeType;

        }
        private void DeSerialized(FileStream file)
        {
            try
            {


                BinaryFormatter formatter = new BinaryFormatter();
                shapes = (List<Shape>)formatter.Deserialize(file);
                Shape.Radius = (int)formatter.Deserialize(file);
                aTimer.Interval = (double)formatter.Deserialize(file);
                aTimer.Enabled = (bool)formatter.Deserialize(file);
                ShapeType = (Type)formatter.Deserialize(file);
                Convex_hull_Type = (int)formatter.Deserialize(file);
                ColorInside = Color.FromArgb((int)formatter.Deserialize(file));
                Shape.Color_of_figure = Color.FromArgb((int)formatter.Deserialize(file));
                file.Close();
            }
            catch
            {
                MessageBox.Show("dll was nit found", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Serialized(FileStream file)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, shapes);
            formatter.Serialize(file, Shape.Radius);
            formatter.Serialize(file, aTimer.Interval);
            formatter.Serialize(file, aTimer.Enabled);
            formatter.Serialize(file, ShapeType);
            formatter.Serialize(file, Convex_hull_Type);
            formatter.Serialize(file, ColorInside.ToArgb());
            formatter.Serialize(file, Shape.Color_of_figure.ToArgb());
            file.Close();
        }
        public void Plugin_Load()
        {
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\plugins");
            string[] Files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\plugins", "*.dll");
            Assembly assembly;
            Type[] types;
            bool New;
            ToolStripMenuItem tool;
            foreach (string name in Files)
            {
                try
                {
                    assembly = Assembly.LoadFrom(name);
                    types = assembly.GetTypes();
                    foreach (Type t in types)
                    {
                        if (t.IsSubclassOf(typeof(Shape)))
                        {
                            if (t.IsPublic)
                            {
                                if (t.IsSerializable)
                                {
                                    if (!t.IsAbstract)
                                    {
                                        New = true;
                                        foreach (ToolStripMenuItem item in shapeToolStripMenuItem.DropDownItems)
                                        {
                                            if (t == (Type)item.Tag)
                                            {
                                                New = !true;
                                                break;
                                            }
                                        }
                                        if (!New)
                                        {
                                            break;
                                        }
                                        assemblies.Add(assembly);

                                        tool = new ToolStripMenuItem();
                                        tool.Text = t.Name;
                                        tool.Tag = t;
                                        shapeToolStripMenuItem.DropDownItems.Add(tool);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

    }


}