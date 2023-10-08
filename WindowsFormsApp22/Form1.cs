using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp22
{
    public partial class Form1 : Form
    {
        OpenFileDialog openFile = new OpenFileDialog();

        public Form1()
        {
            InitializeComponent();
            richTextBox1.AllowDrop = true;
            DirectoryTreeNode();
            TreeView.BeforeExpand += TreeView_BeforeSelect;
            richTextBox1.AllowDrop = true;
            openFile.Filter = "Rich Text Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Добавляем обработчик события выбора узла в TreeView
            TreeView.AfterSelect += TreeView_AfterSelect;

            // Добавляем обработчик события сохранения
            button1.Click += button1_Click;

            // заполняем дерево дисками
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (File.Exists(e.Node.FullPath))
            {
                try
                {
                    if (e.Node.FullPath.EndsWith(".rtf") || e.Node.FullPath.EndsWith(".txt"))
                    {
                        richTextBox1.LoadFile(e.Node.FullPath, RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(e.Node.FullPath, Encoding.Default))
                        {
                            richTextBox1.Text = reader.ReadToEnd();
                        }
                    }
                    openFile.FileName = e.Node.FullPath;
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"{exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode s = (TreeNode)e.Item;
            openFile = new OpenFileDialog();
            DoDragDrop(s.FullPath, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void TreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    string[] dir = Directory.GetDirectories(e.Node.FullPath);
                    for (int i = 0; i < dir.Length; i++)
                    {
                        TreeNode dirNode = new TreeNode(new DirectoryInfo(dir[i]).Name);
                        FillTreeNode(dirNode, dir[i]);
                        e.Node.Nodes.Add(dirNode);
                    }
                    string[] file = Directory.GetFiles(e.Node.FullPath);
                    for (int i = 0; i < file.Length; i++)
                    {
                        TreeNode fileNode = new TreeNode(new DirectoryInfo(file[i]).Name);
                        e.Node.Nodes.Add(fileNode);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Refresh();
            }
        }

        private void DirectoryTreeNode()
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode treeNode = new TreeNode(drive.Name);
                    FillTreeNode(treeNode, drive.Name);
                    TreeView.Nodes.Add(treeNode);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Refresh();
            }
        }

        private void FillTreeNode(TreeNode driveNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode(dir.Remove(0, dir.LastIndexOf("\\") + 1));
                    driveNode.Nodes.Add(dirNode);
                }
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    TreeNode fileNode = new TreeNode(new DirectoryInfo(file).Name);
                    driveNode.Nodes.Add(fileNode);
                }
            }
            catch (Exception)
            {

            }
        }


        private void richTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void richTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            string filePath = e.Data.GetData(DataFormats.Text).ToString();

            if (File.Exists(filePath))
            {
                try
                {
                    if (Path.GetExtension(filePath).Equals(".rtf", StringComparison.OrdinalIgnoreCase))
                    {
                        richTextBox1.LoadFile(filePath, RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
                        {
                            richTextBox1.Text = reader.ReadToEnd();
                        }
                    }
                    openFile.FileName = filePath;
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"{exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(openFile.FileName))
            {
                try
                {
                    if (openFile.FileName.EndsWith(".rtf") || openFile.FileName.EndsWith(".txt"))
                    {
                        richTextBox1.SaveFile(openFile.FileName, RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        File.WriteAllText(openFile.FileName, richTextBox1.Text, Encoding.Default);
                    }
                    MessageBox.Show("Файл успешно сохранен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"{exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
