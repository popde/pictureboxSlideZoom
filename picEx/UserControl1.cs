using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace picEx
{
    public partial class UserControl1: PictureBox 
    {
        public UserControl1()
        {
            InitializeComponent();
            UserControl1_Load();
        }

        private bool isInitParent = false;
        private bool isMouseDown = false;
        private System.Drawing.Point p1 = new System.Drawing.Point();
        private System.Drawing.Point p2 = new System.Drawing.Point();

        public string imgPath { 
            set {
                if (value != string.Empty)
                {
                    this.Size = this.Parent.Size;
                    this.Location = new System.Drawing.Point(0, 0);
                    this.Image = System.Drawing.Image.FromFile(value);
                    //
                    if (!isInitParent)
                    {
                        isInitParent = true;
                        this.Parent.MouseDoubleClick += PictureBox_MouseDoubleClick;
                    }
                }
            }
        }

        private void UserControl1_Load()
        {
            this.Location = new System.Drawing.Point(0, 0);
            this.Dock = DockStyle.None;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.MouseWheel += PictureBox_MouseWheel;
            this.MouseDown += PictureBox_MouseDown;
            this.MouseUp += PictureBox_MouseUp;
            this.MouseMove += PictureBox_MouseMove;
            this.MouseDoubleClick += PictureBox_MouseDoubleClick;
        }

        private void PictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            this.Size = this.Parent.Size;
            this.Location = new System.Drawing.Point(0, 0);
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            //鼠标坐标的相对改变值
            int a = Control.MousePosition.X - p1.X;
            int b = Control.MousePosition.Y - p1.Y;
            //图片坐标计算&赋值
            if (isMouseDown)
                this.Location = new System.Drawing.Point(p2.X + a, p2.Y + b);//图片新的坐标 = 图片起始坐标 + 鼠标相对位移
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            isMouseDown = false;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            isMouseDown = true;
            p1 = Control.MousePosition;//记录鼠标坐标
            p2 = this.Location;                //记录图片坐标
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = this.Width;
            int oh = this.Height;
            int VX, VY; //因缩放产生的位移矢量
            int zoomStep = (this.Width > this.Height) ? this.Width / 10 : this.Height / 10;
            if (e.Delta > 0) //放大
            {
                //防止一直无限放大导致异常错误
                if (Math.Max( this.Width,this.Height) > Math.Max(this.Image.Width * 10, this.Image.Height * 10))
                    return;
                this.Width += zoomStep;
                this.Height += zoomStep;
            }
            if (e.Delta < 0) //缩小
            {
                //防止一直缩
                if ( Math.Min( this.Width,this.Height) < Math.Min( this.Image.Width/10, this.Image.Height / 10) )
                    return;

                this.Width -= zoomStep;
                this.Height -= zoomStep;
            }
            //第④步，求因缩放产生的位移，进行补偿，实现锚点缩放的效果
            VX = (int)((double)x * (ow - this.Width) / ow);
            VY = (int)((double)y * (oh - this.Height) / oh);
            this.Location = new Point(this.Location.X + VX, this.Location.Y + VY);
        }
    }
}
