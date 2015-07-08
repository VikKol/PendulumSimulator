namespace Pendulum
{
    partial class Canvas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._canvas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this._canvas.Location = new System.Drawing.Point(1, -2);
            this._canvas.Name = "_canvas";
            this._canvas.Size = new System.Drawing.Size(741, 521);
            this._canvas.TabIndex = 0;
            this._canvas.TabStop = false;
            // 
            // Canvas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 516);
            this.Controls.Add(this._canvas);
            this.Name = "Canvas";
            this.Text = "Pendulum";
            this.Shown += new System.EventHandler(this.Canvas_Shown);
            ((System.ComponentModel.ISupportInitialize)(this._canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _canvas;
    }
}

