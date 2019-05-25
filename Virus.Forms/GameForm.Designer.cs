using System.Windows.Forms;
using System.Windows;
using System.ComponentModel;
using System;
//using Virus.Core;
using System.Collections.Generic;

namespace Virus.Forms
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;


        //public Game Game;

        

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
            this.MainLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // MainLayout
            // 
            this.MainLayout.Location = new System.Drawing.Point(13, 13);
            this.MainLayout.Name = "MainLayout";
            this.MainLayout.Size = new System.Drawing.Size(618, 273);
            this.MainLayout.TabIndex = 0;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainLayout);
            this.Name = "GameForm";
            this.Text = "GameForm";
            this.ResumeLayout(false);

            //InitGame();
        }

        #endregion

        //private void InitGame()
        //{
        //    Game = new Game(3, 5000, true);

        //    foreach(var p in Game.Players)
        //    {
        //        FlowLayoutPanel flp = new FlowLayoutPanel();
        //        PlayerPanels.Add(flp);
        //        TextBox tb = new TextBox();
        //        tb.Text = p.ShortDescription;
        //        flp.Controls.Add(tb);
        //        MainLayout.Controls.Add(flp);
        //    }
        //}


        private List<Panel> PlayerPanels = new List<Panel>(); 


        private FlowLayoutPanel MainLayout;
    }
}