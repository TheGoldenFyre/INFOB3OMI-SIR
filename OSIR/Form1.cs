namespace OSIR
{
    public partial class Form1 : Form
    {
        static OSIRModel model = new OSIRModel(100, 100, 0.03, 0.10, 0.03);

        Label label = new Label();
        Button btn = new Button();
        Button runbtn = new Button();
        static PictureBox pb = new PictureBox();
        static bool threadRunning = false;
        Thread runthr;

        public Form1()
        {
            model.AddInfections(0.1);
            model.AddObstructions(1.0);
            this.Size = new Size(model.GetBitmap().Width, model.GetBitmap().Height);
            pb.Location = new Point(0, 0);
            pb.Visible = true;
            pb.Size = new Size(model.GetBitmap().Width, model.GetBitmap().Height);
            btn.Location = new Point(5, 5);
            btn.Text = "Advance";
            btn.Size = new Size(100, 25);
            runbtn.Location = new Point(5, 35);
            runbtn.Text = "Run";
            runbtn.Size = new Size(100, 25);
            btn.Click += (s, e) => { 
                model.Iterate(); 
                if (pb.Image != null) pb.Image.Dispose();
                Bitmap bm = model.GetBitmap();
                pb.Image = bm.Clone(new Rectangle(0, 0, bm.Width, bm.Height), System.Drawing.Imaging.PixelFormat.DontCare);
                pb.Invalidate();
                Invalidate();
            };
            runbtn.Click += (s, e) =>
            {
                if (threadRunning)
                    threadRunning = false;
                else
                {
                    threadRunning = true;
                    runthr = new Thread(threadAction);
                    runthr.Start();
                }
            };
            //Controls.Add(label);
            Controls.Add(btn);
            Controls.Add(runbtn);
            Controls.Add(pb);
            //model.AddInfections(10);
            //label.Text = model.ToString();
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.FormClosed += (e, f) => { if (threadRunning && runthr != null) threadRunning=false; };
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            label.Text = "???";
            Invalidate();
            if (e.KeyCode == Keys.Space)
            {
                model.AddInfections(10);
                label.Text = model.ToString();
                //counter++;
                //Iterate();
                //UpdateDisplay();
            }
        }

        private void threadAction()
        {
            while (threadRunning)
            {
                model.Iterate();
                Bitmap bm = model.GetBitmap();
                try
                {
                    pb.Invoke((MethodInvoker)delegate
                    {
                        if (threadRunning)
                        {
                            if (pb.Image != null) pb.Image.Dispose();
                            pb.Image = bm.Clone(new Rectangle(0, 0, bm.Width, bm.Height), System.Drawing.Imaging.PixelFormat.DontCare);
                            pb.Invalidate();
                        }
                    });

                } catch (Exception ex) { /* Oopsy whopsy */ }
            }
        }
    }
}