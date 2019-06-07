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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.pUserHand = new System.Windows.Forms.FlowLayoutPanel();
            this.bDiscard = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lTurns = new System.Windows.Forms.Label();
            this.tbGame = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pUserHand.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pUserHand
            // 
            this.pUserHand.AutoScroll = true;
            this.pUserHand.AutoSize = true;
            this.pUserHand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pUserHand.Controls.Add(this.bDiscard);
            this.pUserHand.Controls.Add(this.button1);
            this.pUserHand.Controls.Add(this.lTurns);
            this.pUserHand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pUserHand.Location = new System.Drawing.Point(449, 335);
            this.pUserHand.Name = "pUserHand";
            this.pUserHand.Size = new System.Drawing.Size(438, 325);
            this.pUserHand.TabIndex = 1;
            // 
            // bDiscard
            // 
            this.bDiscard.Location = new System.Drawing.Point(3, 3);
            this.bDiscard.Name = "bDiscard";
            this.bDiscard.Size = new System.Drawing.Size(189, 36);
            this.bDiscard.TabIndex = 0;
            this.bDiscard.Text = "Begin to discard";
            this.bDiscard.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(198, 3);
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
            this.lTurns.Location = new System.Drawing.Point(367, 0);
            this.lTurns.Name = "lTurns";
            this.lTurns.Size = new System.Drawing.Size(51, 20);
            this.lTurns.TabIndex = 3;
            this.lTurns.Text = "label1";
            // 
            // tbGame
            // 
            this.tbGame.BackColor = System.Drawing.SystemColors.InfoText;
            this.tbGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbGame.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbGame.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.tbGame.Location = new System.Drawing.Point(449, 4);
            this.tbGame.Multiline = true;
            this.tbGame.Name = "tbGame";
            this.tbGame.ReadOnly = true;
            this.tbGame.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbGame.Size = new System.Drawing.Size(438, 324);
            this.tbGame.TabIndex = 4;
            this.tbGame.WordWrap = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.tbLog, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbGame, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pUserHand, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.MainLayout, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(100, 50);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1338, 664);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // tbLog
            // 
            this.tbLog.BackColor = System.Drawing.SystemColors.InfoText;
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLog.ForeColor = System.Drawing.Color.Lime;
            this.tbLog.Location = new System.Drawing.Point(894, 4);
            this.tbLog.MaxLength = 999999999;
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tableLayoutPanel1.SetRowSpan(this.tbLog, 2);
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(440, 656);
            this.tbLog.TabIndex = 5;
            // 
            // MainLayout
            // 
            this.MainLayout.AutoScroll = true;
            this.MainLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.MainLayout.ColumnCount = 2;
            this.MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayout.Location = new System.Drawing.Point(4, 4);
            this.MainLayout.Name = "MainLayout";
            this.MainLayout.RowCount = 2;
            this.tableLayoutPanel1.SetRowSpan(this.MainLayout, 2);
            this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainLayout.Size = new System.Drawing.Size(438, 656);
            this.MainLayout.TabIndex = 6;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1338, 664);
            this.Controls.Add(this.tableLayoutPanel1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.MinimumSize = new System.Drawing.Size(1360, 720);
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameForm";
            this.pUserHand.ResumeLayout(false);
            this.pUserHand.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel pUserHand;

        private Button button1;
        private Label lTurns;
        private TextBox tbGame;
        private Button bDiscard;
        private TableLayoutPanel tableLayoutPanel1;
        protected TextBox tbLog;
        private TableLayoutPanel MainLayout;
    }
}