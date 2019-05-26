using System.Windows.Forms;
using System.Windows;
using System.ComponentModel;
using System;
using Virus.Core;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Virus.Forms
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        
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
            Application.Exit();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.pUserHand = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.lTurns = new System.Windows.Forms.Label();
            this.tbGame = new System.Windows.Forms.TextBox();
            this.bDiscard = new System.Windows.Forms.Button();
            this.pUserHand.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayout
            // 
            this.MainLayout.AutoScroll = true;
            this.MainLayout.AutoSize = true;
            this.MainLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.MainLayout.Location = new System.Drawing.Point(13, 13);
            this.MainLayout.Name = "MainLayout";
            this.MainLayout.Size = new System.Drawing.Size(618, 639);
            this.MainLayout.TabIndex = 0;
            // 
            // pUserHand
            // 
            this.pUserHand.AutoScroll = true;
            this.pUserHand.AutoSize = true;
            this.pUserHand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pUserHand.Controls.Add(this.bDiscard);
            this.pUserHand.Location = new System.Drawing.Point(637, 393);
            this.pUserHand.Name = "pUserHand";
            this.pUserHand.Size = new System.Drawing.Size(695, 259);
            this.pUserHand.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(903, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 40);
            this.button1.TabIndex = 2;
            this.button1.Text = "Pass turn";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lTurns
            // 
            this.lTurns.AutoSize = true;
            this.lTurns.Location = new System.Drawing.Point(638, 13);
            this.lTurns.Name = "lTurns";
            this.lTurns.Size = new System.Drawing.Size(51, 20);
            this.lTurns.TabIndex = 3;
            this.lTurns.Text = "label1";
            // 
            // tbGame
            // 
            this.tbGame.Location = new System.Drawing.Point(637, 73);
            this.tbGame.Multiline = true;
            this.tbGame.Name = "tbGame";
            this.tbGame.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbGame.Size = new System.Drawing.Size(695, 314);
            this.tbGame.TabIndex = 4;
            // 
            // bDiscard
            // 
            this.bDiscard.Location = new System.Drawing.Point(3, 3);
            this.bDiscard.Name = "bDiscard";
            this.bDiscard.Size = new System.Drawing.Size(189, 36);
            this.bDiscard.TabIndex = 0;
            this.bDiscard.Text = "Discard selected";
            this.bDiscard.UseVisualStyleBackColor = true;
            this.bDiscard.Click += new System.EventHandler(this.bDiscard_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 664);
            this.Controls.Add(this.tbGame);
            this.Controls.Add(this.lTurns);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pUserHand);
            this.Controls.Add(this.MainLayout);
            this.Name = "GameForm";
            this.Text = "GameForm";
            this.pUserHand.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        private FlowLayoutPanel MainLayout;
        private FlowLayoutPanel pUserHand;

        private Button button1;
        private Label lTurns;
        private TextBox tbGame;
        private Button bDiscard;
    }
}