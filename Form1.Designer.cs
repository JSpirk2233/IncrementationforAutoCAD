
namespace IncrementationProgram
{
    partial class Form1
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
            this.incUp = new System.Windows.Forms.RadioButton();
            this.incDown = new System.Windows.Forms.RadioButton();
            this.increment = new System.Windows.Forms.TextBox();
            this.upByOne = new System.Windows.Forms.Button();
            this.downByOne = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // incUp
            // 
            this.incUp.AutoSize = true;
            this.incUp.Checked = true;
            this.incUp.Location = new System.Drawing.Point(13, 13);
            this.incUp.Name = "incUp";
            this.incUp.Size = new System.Drawing.Size(89, 17);
            this.incUp.TabIndex = 0;
            this.incUp.TabStop = true;
            this.incUp.Text = "Increment Up";
            this.incUp.UseVisualStyleBackColor = true;
            this.incUp.CheckedChanged += new System.EventHandler(this.incUp_CheckedChanged);
            // 
            // incDown
            // 
            this.incDown.AutoSize = true;
            this.incDown.Location = new System.Drawing.Point(13, 36);
            this.incDown.Name = "incDown";
            this.incDown.Size = new System.Drawing.Size(103, 17);
            this.incDown.TabIndex = 1;
            this.incDown.Text = "Increment Down";
            this.incDown.UseVisualStyleBackColor = true;
            this.incDown.CheckedChanged += new System.EventHandler(this.incDown_CheckedChanged);
            // 
            // increment
            // 
            this.increment.Location = new System.Drawing.Point(153, 23);
            this.increment.Name = "increment";
            this.increment.Size = new System.Drawing.Size(51, 20);
            this.increment.TabIndex = 2;
            this.increment.TextChanged += new System.EventHandler(this.increment_TextChanged);
            // 
            // upByOne
            // 
            this.upByOne.Location = new System.Drawing.Point(211, 23);
            this.upByOne.Name = "upByOne";
            this.upByOne.Size = new System.Drawing.Size(22, 20);
            this.upByOne.TabIndex = 3;
            this.upByOne.Text = "▲";
            this.upByOne.UseVisualStyleBackColor = true;
            this.upByOne.Click += new System.EventHandler(this.upByOne_Click);
            // 
            // downByOne
            // 
            this.downByOne.Location = new System.Drawing.Point(125, 23);
            this.downByOne.Name = "downByOne";
            this.downByOne.Size = new System.Drawing.Size(22, 20);
            this.downByOne.TabIndex = 4;
            this.downByOne.Text = "▼";
            this.downByOne.UseVisualStyleBackColor = true;
            this.downByOne.Click += new System.EventHandler(this.downByOne_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 73);
            this.Controls.Add(this.downByOne);
            this.Controls.Add(this.upByOne);
            this.Controls.Add(this.increment);
            this.Controls.Add(this.incDown);
            this.Controls.Add(this.incUp);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton incUp;
        private System.Windows.Forms.RadioButton incDown;
        private System.Windows.Forms.TextBox increment;
        private System.Windows.Forms.Button upByOne;
        private System.Windows.Forms.Button downByOne;
    }
}