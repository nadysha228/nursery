namespace Nursey2
{
    partial class Volonter
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
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.button1_Add = new System.Windows.Forms.Button();
            this.button2_Delete = new System.Windows.Forms.Button();
            this.button3_Exit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(26, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(527, 47);
            this.label1.TabIndex = 2;
            this.label1.Text = "Данные об усыновлении";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(34, 101);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1364, 508);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button1_Add
            // 
            this.button1_Add.BackColor = System.Drawing.Color.White;
            this.button1_Add.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1_Add.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.button1_Add.Location = new System.Drawing.Point(34, 653);
            this.button1_Add.Name = "button1_Add";
            this.button1_Add.Size = new System.Drawing.Size(374, 56);
            this.button1_Add.TabIndex = 9;
            this.button1_Add.Text = "Добавить";
            this.button1_Add.UseVisualStyleBackColor = false;
            this.button1_Add.Click += new System.EventHandler(this.button1_Add_Click);
            // 
            // button2_Delete
            // 
            this.button2_Delete.BackColor = System.Drawing.Color.White;
            this.button2_Delete.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2_Delete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.button2_Delete.Location = new System.Drawing.Point(436, 653);
            this.button2_Delete.Name = "button2_Delete";
            this.button2_Delete.Size = new System.Drawing.Size(374, 56);
            this.button2_Delete.TabIndex = 10;
            this.button2_Delete.Text = "Удалить";
            this.button2_Delete.UseVisualStyleBackColor = false;
            this.button2_Delete.Click += new System.EventHandler(this.button2_Delete_Click);
            // 
            // button3_Exit
            // 
            this.button3_Exit.BackColor = System.Drawing.Color.White;
            this.button3_Exit.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3_Exit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.button3_Exit.Location = new System.Drawing.Point(1179, 653);
            this.button3_Exit.Name = "button3_Exit";
            this.button3_Exit.Size = new System.Drawing.Size(239, 56);
            this.button3_Exit.TabIndex = 11;
            this.button3_Exit.Text = "Выход";
            this.button3_Exit.UseVisualStyleBackColor = false;
            this.button3_Exit.Click += new System.EventHandler(this.button3_Exit_Click);
            // 
            // Volonter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(159)))), ((int)(((byte)(223)))));
            this.ClientSize = new System.Drawing.Size(1569, 837);
            this.Controls.Add(this.button3_Exit);
            this.Controls.Add(this.button2_Delete);
            this.Controls.Add(this.button1_Add);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label1);
            this.Name = "Volonter";
            this.Text = "Volonter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button1_Add;
        private System.Windows.Forms.Button button2_Delete;
        private System.Windows.Forms.Button button3_Exit;
    }
}