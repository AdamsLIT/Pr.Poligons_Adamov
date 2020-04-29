using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapes;

namespace Адамов_Никита_проект_10_класса
{
    interface  IAction
    {

         void ReDo(Form1 form1);
         void UnDo(Form1 form1);

    }

    class AddShape : IAction
    {
        Shape figure;
        int index;
        public AddShape(Shape shape, int index )
        {
            figure = shape;
            this.index = index;

        }
        public  void ReDo(Form1 form1)
        {
            form1.AddShape(figure, index);
        }
        public void UnDo(Form1 form1)
        {
            form1.DeleteShape(index);
        }
       

    }
    class DeleteShape: IAction
    {
        Shape figure;
        int index;
        public DeleteShape(Shape shape, int index)
        {
            figure = shape;
            this.index = index;

        }
        public void ReDo(Form1 form1)
        {
            form1.DeleteShape(index);
           
        }
        public void UnDo(Form1 form1)
        {
            form1.AddShape(figure, index);
        }


    }
    class Moving : IAction
    {
        int x,y,index;
        public Moving(int x, int y, int index)
        {
            this.x = x;
            this.y = y;
            this.index = index;
        }
        public void ReDo(Form1 form1)
        {
            form1.Add_XY_to_Shape(x, y, index);
        }

        public void UnDo(Form1 form1)
        {
            form1.Add_XY_to_Shape(-x, -y, index);
        }
    }
    class Radius_changes : IAction
    {
        int Rad_Delta;
        public Radius_changes(int delta)
        {
            Rad_Delta = delta;
        }
        public void ReDo(Form1 form1)
        {
            form1.Add_to_Radius(Rad_Delta);
        }

        public void UnDo(Form1 form1)
        {
            form1.Add_to_Radius(-Rad_Delta);
        }
    }
    class Color_changes_hull : IAction
    {
        Color ColorInside_old,ColorInside_new ;
        public Color_changes_hull(Color ColorInside_new ,Color ColorInside_old)
        {
            this.ColorInside_old = ColorInside_old;
            this.ColorInside_new = ColorInside_new;

        }
        public void ReDo(Form1 form1)
        {
            form1.Return_Color_hull(ColorInside_new);
        }

        public void UnDo(Form1 form1)
        {
            form1.Return_Color_hull(ColorInside_old);
        }
    }
    class Color_changes_figures : IAction
    {
        Color ColorOfigure_old, ColorOfigure_new;
        public Color_changes_figures(Color ColorOfigure_new, Color ColorOfigure_old)
        {
            this.ColorOfigure_new = ColorOfigure_new;
            this.ColorOfigure_old = ColorOfigure_old;

        }
        public void ReDo(Form1 form1)
        {
            form1.Return_Color_figure(ColorOfigure_new);
        }

        public void UnDo(Form1 form1)
        {
            form1.Return_Color_figure(ColorOfigure_old);
        }
    }
    class Figure_Choosen : IAction
    {
        Type Shape_type_Old, Shape_type_New;
        public Figure_Choosen(Type Shape_type_old, Type Shape_type_new)
        {
            Shape_type_New = Shape_type_new;
            Shape_type_Old = Shape_type_old;


        }
        public void ReDo(Form1 form1)
        {
            form1.Return_Type_figure(Shape_type_New);
        }

        public void UnDo(Form1 form1)
        {
            form1.Return_Type_figure(Shape_type_Old);
        }
    }
}
