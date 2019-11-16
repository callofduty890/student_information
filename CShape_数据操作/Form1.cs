using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace CShape_数据操作
{
    public partial class Form1 : Form
    {

        public class XmlConfigUtil
        {
            #region 全局变量
            string _xmlPath;        //文件所在路径
            #endregion

            #region 构造函数
            /// <summary>
            /// 初始化一个配置
            /// </summary>
            /// <param name="xmlPath">配置所在路径</param>
            public XmlConfigUtil(string xmlPath)
            {
                _xmlPath = Path.GetFullPath(xmlPath);
            }
            #endregion

            #region 公有方法
            /// <summary>
            /// 写入配置
            /// </summary>
            /// <param name="value">写入的值</param>
            /// <param name="nodes">节点</param>
            public void Write(string value, params string[] nodes)
            {
                //初始化xml
                XmlDocument xmlDoc = new XmlDocument();
                if (File.Exists(_xmlPath))
                    xmlDoc.Load(_xmlPath);
                else
                    xmlDoc.LoadXml("<XmlConfig />");
                XmlNode xmlRoot = xmlDoc.ChildNodes[0];

                //新增、编辑 节点
                string xpath = string.Join("/", nodes);
                XmlNode node = xmlDoc.SelectSingleNode(xpath);
                if (node == null)    //新增节点
                {
                    node = makeXPath(xmlDoc, xmlRoot, xpath);
                }
                node.InnerText = value;

                //保存
                xmlDoc.Save(_xmlPath);
            }

            /// <summary>
            /// 读取配置
            /// </summary>
            /// <param name="nodes">节点</param>
            /// <returns></returns>
            public string Read(params string[] nodes)
            {
                XmlDocument xmlDoc = new XmlDocument();
                if (File.Exists(_xmlPath) == false)
                    return null;
                else
                    xmlDoc.Load(_xmlPath);

                string xpath = string.Join("/", nodes);
                XmlNode node = xmlDoc.SelectSingleNode("/XmlConfig/" + xpath);
                if (node == null)
                    return null;

                return node.InnerText;
            }
            #endregion

            #region 私有方法
            //递归根据 xpath 的方式进行创建节点
            static private XmlNode makeXPath(XmlDocument doc, XmlNode parent, string xpath)
            {

                // 在XPath抓住下一个节点的名称；父级如果是空的则返回
                string[] partsOfXPath = xpath.Trim('/').Split('/');
                string nextNodeInXPath = partsOfXPath.First();
                if (string.IsNullOrEmpty(nextNodeInXPath))
                    return parent;

                // 获取或从名称创建节点
                XmlNode node = parent.SelectSingleNode(nextNodeInXPath);
                if (node == null)
                    node = parent.AppendChild(doc.CreateElement(nextNodeInXPath));

                // 加入的阵列作为一个XPath表达式和递归余数
                string rest = String.Join("/", partsOfXPath.Skip(1).ToArray());
                return makeXPath(doc, node, rest);
            }
            #endregion
        }

        //创建学生信息的类
        [Serializable]
        class student_information
        {
            public string student_name;
            public string student_class;
            public string phone_number;
            public string student_year;
        }
            
        public Form1()
        {
            InitializeComponent();
        }
        //序列化 数据： 内存->硬盘
        private void button1_Click(object sender, EventArgs e)
        {
            //实例化对象
            student_information student = new student_information()
            {
                student_name = this.textBox1.Text.Trim(),
                student_class = this.textBox2.Text.Trim(),
                student_year = this.textBox3.Text.Trim(),
                phone_number = this.textBox4.Text.Trim()
            };
            //创建文件流
            FileStream fs = new FileStream("objstudent.stu", FileMode.Create);
            //二进制->创建操作对象
            BinaryFormatter formatter = new BinaryFormatter();
            //调用 序列化方法
            formatter.Serialize(fs, student);
            //关闭文件流
            fs.Close();
        }

        //反序列化
        private void button2_Click(object sender, EventArgs e)
        {
            //创建文件流
            FileStream fs = new FileStream("objstudent.stu", FileMode.Open);
            //二进制->创建操作对象
            BinaryFormatter formatter = new BinaryFormatter();
            //使用反序列化
            student_information objstudent = (student_information)formatter.Deserialize(fs);
            //关闭文件流
            fs.Close();
            //界面显示
            this.textBox1.Text = objstudent.student_name;
            this.textBox2.Text = objstudent.student_class;
            this.textBox3.Text = objstudent.student_year;
            this.textBox4.Text = objstudent.phone_number;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //初始化并指定文件路径
            XmlConfigUtil util = new XmlConfigUtil("配置信息.xml");
            //写入数据
            util.Write(this.textBox8.Text.Trim(),"information", "student_name");
        }
    }
}
