using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using Microsoft.VisualBasic;
using GraphSpace;
using DBNamespace;
using locationSpace;
using ServiceSpace;
namespace eMap
{

    public partial class Form1 : Form
    {
        // string names, pkey;

        LocationNode graph;
        databaseConnectivity db;
        int[] pathArray;
        int source=-1, dest=-1,scrollFlag=0;
        Image sourceImg, destImg,dummyImg;
        bool flag = false,refreshflag=false;
        Location[] locationArray;
        Services[] serviceArray;

        int locCount;
        string sourceString = " ";
         LinkedList<int> allpathvertices;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            sourceImg = Image.FromFile("SrcImage.jpg");
            destImg = Image.FromFile("DestImage.jpg");
            dummyImg = Image.FromFile("DummyImage.jpg");
            db = new databaseConnectivity();
            db.connectToDatabase();
            allpathvertices = new LinkedList<int>();
            locCount = db.getLocationCount();
            locationArray = new Location[locCount];
            if (locationArray == null)
                MessageBox.Show("Memory not allocated");
            db.readLocation(locationArray);
            pathArray = new int[locCount];
            graph = new LocationNode(locCount);
            db.readEdgeTable(graph, locationArray);
            //string str = " ";
            serviceArray = new Services[locCount];
            db.readService(serviceArray);
            var servicestring = new AutoCompleteStringCollection();
            for (int i = 0; i < serviceArray.Length; i++)
                if (!(serviceArray[i] == null||serviceArray[i].service.CompareTo("nil")==0))
                {
                    servicestring.Add(serviceArray[i].service);
                    comboBox3.Items.Add(serviceArray[i].service);
                }  
            var autostring = new AutoCompleteStringCollection();
            for (int i = 0; i < locCount; i++)
            {
                comboBox1.Items.Add(locationArray[i].locName);
                comboBox2.Items.Add(locationArray[i].locName);

                autostring.Add(locationArray[i].locName);
            }
            comboBox1.AutoCompleteCustomSource = autostring;
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            panel1.SetAutoScrollMargin (2,3);

            comboBox2.AutoCompleteCustomSource = autostring;
            comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox2.AutoCompleteSource = AutoCompleteSource.CustomSource;

            comboBox3.AutoCompleteCustomSource = servicestring;
            comboBox3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;

            //Tool tip to help how to use the form
            ToolTip toolTip1 = new ToolTip();
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(comboBox1, "Insert source to use showpath button.");
            ToolTip toolTip2 = new ToolTip();
            toolTip2.ShowAlways = true;
            toolTip2.SetToolTip(comboBox2, "Insert destination to use showpath button.");
            ToolTip toolTip3 = new ToolTip();
            toolTip3.ShowAlways = true;
            toolTip3.SetToolTip(comboBox3, "Insert destination type to use showallpath button.");
            ToolTip toolTip4 = new ToolTip();
            toolTip4.ShowAlways = true;
            toolTip4.SetToolTip(button3, "Select source and destination to get the path.");
            ToolTip toolTip5 = new ToolTip();
            toolTip5.ShowAlways = true;
            toolTip5.SetToolTip(button20, "Select source and destination type to get all paths.");
           }


        void refreshOnScroll(object sender,ScrollEventArgs arg)
        {
            if (scrollFlag == 1)
            {
                showPath();
                             
            }
            else if (scrollFlag == 2)
            {
                showAllPath();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //databaseConnectivity db = new databaseConnectivity();
            //db.connectToDatabase();
            //db.insertToLocation(4, "zircon", button2.Location.X, button2.Location.Y);

        }

        void fetchAllPathVertices()
        {
            for(int index=0;index<serviceArray.Length;index++)
                if(serviceArray[index]!=null)
                {
                    if ((sourceString.CompareTo("food") == 0) && (serviceArray[index].service.CompareTo("canteen") == 0 || serviceArray[index].service.CompareTo("mess") == 0))
                        allpathvertices.AddFirst(serviceArray[index].locId);
                    else if(sourceString.CompareTo(serviceArray[index].service)==0)
                        allpathvertices.AddFirst(serviceArray[index].locId);
                    ;
        }
        }
    



        void refreshPanel()
        {
            panel1.Invalidate();
        }
        private void button3_Click(object sender, EventArgs e)
        {
          
            if (refreshflag == false&&source!=dest&&(source!=-1||dest!=-1))
            {
                graph.dijkstra(pathArray, source);
                showPath();
                button3.Text = "ReFresh";
                refreshflag = true;
                scrollFlag = 1;
            }
            else
            {
                button3.Text = "ShowPath";
                refreshflag = false;
                refreshPanels();
                scrollFlag = 0;
           
          
            }
            

            
        }


        void showPath()
        {
           
            int reach = pathArray[dest];
            LinkedList<int> list = new LinkedList<int>();
            list.AddFirst(dest);
            while (reach != source)
            {
                list.AddFirst(reach);
                reach = pathArray[reach];

            }
            list.AddFirst(reach);
            int prev = -1;
            foreach (int i in list)
            {
                if (prev == -1)
                    prev = i;
                else
                {
                    drawLineBetweenPoints(locationArray[prev].xcord, locationArray[prev].ycord, locationArray[i].xcord, locationArray[i].ycord);
                    prev = i;
                }

            }

            flag = false;
            //button3.Enabled = false;

        }

        void showAllPath()
        {
            fetchAllPathVertices();
            graph.dijkstra(pathArray, source);
           
            //for (int index = 0; index < destArray.Length; index++)
            foreach (int index in allpathvertices)
            {
                dest = index;//destArray[index];
                showPath();
            }
            allpathvertices.Clear();
        }


        

        public void drawLineBetweenPoints(int sourcex, int sourcey, int destx, int desty)
        {
            System.Drawing.Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.GreenYellow,6);
            System.Drawing.Graphics formGraphics = panel1.CreateGraphics();

            formGraphics.DrawLine(myPen, sourcex, sourcey, destx, desty);
            myPen.Dispose();
             formGraphics.Dispose();
        }        

        private void label4_Click(object sender, EventArgs e)
        {
            setAquamarine();

        }
        void setAquamarine()
        {
            if (flag == false)
            {
                source = 0;
                flag = true;
                label4.Image = sourceImg;
            }
            else
            {
                dest = 0;
                label4.Image = destImg;
            }
        }


        private void label5_Click(object sender, EventArgs e)
        {
            setIce();
        }
        void setIce()
        {
            if (flag == false)
            {
                source = 49;
                flag = true;
                label5.Image = sourceImg;
            }
            else
            {
                dest = 49;
                label5.Image = destImg;
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            if (flag == false)
            {
                source = 3;
                flag = true;
                label6.Image = sourceImg;
            }
            else
            {
                dest = 3;
                label6.Image = destImg;
            }
        }





        private void label7_Click(object sender, EventArgs e)
        {
            setMegaMess2();


        }
        void setMegaMess2()
        {

            if (flag == false)
            {
                source = 6;
                flag = true;
                label7.Image = sourceImg;
            }
            else
            {
                dest = 6;
                label7.Image = destImg;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            
        }

        void refreshPanels()
        {
            source = dest = -1;
           
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            flag = false;
            button3.Enabled = true;
            label2.Image = dummyImg;
            label3.Image = dummyImg;
            label4.Image = dummyImg;
            label5.Image = dummyImg;
            label6.Image = dummyImg;
            label7.Image = dummyImg;
            label8.Image = dummyImg;
            label9.Image = dummyImg;
            label72.Image = dummyImg;
            label73.Image = dummyImg;
            
            label10.Image = dummyImg;
            label11.Image = dummyImg;
            label12.Image = dummyImg;
            label13.Image = dummyImg;
            label14.Image = dummyImg;
            label15.Image = dummyImg;
            label16.Image = dummyImg;
            label17.Image = dummyImg;
            label18.Image = dummyImg;
            label19.Image = dummyImg;
            label20.Image = dummyImg;
            label21.Image = dummyImg;
            label22.Image = dummyImg;
            label23.Image = dummyImg;
            label24.Image = dummyImg;
            label25.Image = dummyImg;
            label26.Image = dummyImg;
            label27.Image = dummyImg;
            label28.Image = dummyImg;
            label29.Image = dummyImg;
            label30.Image = dummyImg;
            label31.Image = dummyImg;
            label32.Image = dummyImg;
            label33.Image = dummyImg;
            label34.Image = dummyImg;
            label35.Image = dummyImg;
            label36.Image = dummyImg;
            label37.Image = dummyImg;
            label38.Image = dummyImg;
            label39.Image = dummyImg;
            label40.Image = dummyImg;
            label41.Image = dummyImg;
            label42.Image = dummyImg;
            label43.Image = dummyImg;
            label44.Image = dummyImg;
            label45.Image = dummyImg;
            label46.Image = dummyImg;
            label47.Image = dummyImg;
            label48.Image = dummyImg;
            label49.Image = dummyImg;
            label50.Image = dummyImg;
            label51.Image = dummyImg;
            label52.Image = dummyImg;
            label53.Image = dummyImg;
            label54.Image = dummyImg;
            label55.Image = dummyImg;
            label56.Image = dummyImg;
            label57.Image = dummyImg;
            label58.Image = dummyImg;
            label59.Image = dummyImg;
            label60.Image = dummyImg;
            label61.Image = dummyImg;
            
            label66.Image = dummyImg;
            label68.Image = dummyImg;
            refreshPanel();
            
        }
       /* private void button18_Click(object sender, EventArgs e)
        {
            databaseConnectivity db = new databaseConnectivity();
            db.connectToDatabase();
            db.insertToLocation(88, "OPALMainGate", button18.Location.X, button18.Location.Y);
        }*/

       

        private void label8_Click(object sender, EventArgs e)
        {
            setGarnet();
        }
        void setGarnet()
        {
            if (flag == false)
            {
                source = 19;
                flag = true;
                label8.Image = sourceImg;
            }
            else
            {
                dest = 19;
                label8.Image = destImg;
            }
        }
        private void label9_Click(object sender, EventArgs e)
        {
            setZirconA();
           
        }
        void setZirconA()
        {
            if (flag == false)
            {
                source = 5;
                flag = true;
                label9.Image = sourceImg;
            }
            else
            {
                dest = 5;
                label9.Image = destImg;
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {
            setPearl();
        }

        void setPearl()
        {
            if (flag == false)
            {
                source = 15;
                flag = true;
                label10.Image = sourceImg;
            }
            else
            {
                dest = 15;
                label10.Image = destImg;
            }
        }
        private void label11_Click(object sender, EventArgs e)
        {
            setHospital();
        }
        void setHospital()
        {
            if (flag == false)
            {
                source = 31;
                flag = true;
                label11.Image = sourceImg;
            }
            else
            {
                dest = 31;
                label11.Image = destImg;
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {
            setZirconC();
        }
        void setZirconC()
        {
            if (flag == false)
            {
                source = 5;
                flag = true;
                label12.Image = sourceImg;
            }
            else
            {
                dest = 5;
                label12.Image = destImg;
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {
            setZirconB();
        }
        void setZirconB()
        {
            if (flag == false)
            {
                source = 5;
                flag = true;
                label13.Image = sourceImg;
            }
            else
            {
                dest = 5;
                label13.Image = destImg;
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            if (flag == false)
            {
                source = 9;
                flag = true;
                label14.Image = sourceImg;
            }
            else
            {
                dest = 9;
                label14.Image = destImg;
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {
            setSaphire();
        }
        void setSaphire()
        {
            if (flag == false)
            {
                source = 8;
                flag = true;
                label15.Image = sourceImg;
            }
            else
            {
                dest = 8;
                label15.Image = destImg;
            }
        }


        private void label16_Click(object sender, EventArgs e)
        {
            setTopaz();
        }
        void setTopaz()
        {
            if (flag == false)
            {
                source = 10;
                flag = true;
                label16.Image = sourceImg;
            }
            else
            {
                dest = 10;
                label16.Image = destImg;
            }
        }
        private void label18_Click(object sender, EventArgs e)
        {
            setRuby();
        }
        void setRuby()
        {
            if (flag == false)
            {
                source = 27;
                flag = true;
                label18.Image = sourceImg;
            }
            else
            {
                dest = 27;
                label18.Image = destImg;
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {
            if (flag == false)
            {
                source = 99;
                flag = true;
                label17.Image = sourceImg;
            }
            else
            {
                dest = 99;
                label17.Image = destImg;
            }
        }

        private void label35_Click(object sender, EventArgs e)
        {
            setSBIATM();
        }

        void setSBIATM()
        {
            if (flag == false)
            {
                source = 49;
                flag = true;
                label35.Image = sourceImg;
            }
            else
            {
                dest = 49;
                label35.Image = destImg;
            }
        }

        private void label57_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {
            setDiamond();
        }
        void setDiamond()
        {
            if (flag == false)
            {
                source = 21;
                flag = true;
                label28.Image = sourceImg;
            }
            else
            {
                dest = 21;
                label28.Image = destImg;
            }
        }

        private void label27_Click(object sender, EventArgs e)
        {
            setEmerald();
        }

        void setEmerald()
        {
            if (flag == false)
            {
                source = 23;
                flag = true;
                label27.Image = sourceImg;
            }
            else
            {
                dest = 23;
                label27.Image = destImg;
            }
        }

        private void label26_Click(object sender, EventArgs e)
        {
            setJade();
        }
        void setJade()
        {
            if (flag == false)
            {
                source = 25;
                flag = true;
                label26.Image = sourceImg;
            }
            else
            {
                dest =25;
                label26.Image = destImg;
            }
        }

        private void label30_Click(object sender, EventArgs e)
        {
            setGarnetB();
        }
        void setGarnetB()
        {
            if (flag == false)
            {
                source = 28;
                flag = true;
                label30.Image = sourceImg;
            }
            else
            {
                dest = 28;
                label30.Image = destImg;
            }
        }

        private void label25_Click(object sender, EventArgs e)
        {
            setAgate();
        }
        void setAgate()
        {
            if (flag == false)
            {
                source = 33;
                flag = true;
                label25.Image = sourceImg;
            }
            else
            {
                dest = 33;
                label25.Image = destImg;
            }
        }

        private void label36_Click(object sender, EventArgs e)
        {
            setHostelOffice();
        }
        void setHostelOffice()
        {
            if (flag == false)
            {
                source = 29;
                flag = true;
                label36.Image = sourceImg;
            }
            else
            {
                dest = 29;
                label36.Image = destImg;
            }
        }

        private void label24_Click(object sender, EventArgs e)
        {
            setGurunath();
        }
        void setGurunath()
        {
            if (flag == false)
            {
                source = 30;
                flag = true;
                label24.Image = sourceImg;
            }
            else
            {
                dest = 30;
                label24.Image = destImg;
            }
        }

        private void label23_Click(object sender, EventArgs e)
        {
            setCoral();
        }
        void setCoral()
        {
            if (flag == false)
            {
                source = 36;
                flag = true;
                label23.Image = sourceImg;
            }
            else
            {
                dest = 36;
                label23.Image = destImg;
            }
        }

        private void label22_Click(object sender, EventArgs e)
        {
            setBeryl();
        }
        void setBeryl()
        {
            if (flag == false)
            {
                source = 38;
                flag = true;
                label22.Image = sourceImg;
            }
            else
            {
                dest = 38;
                label22.Image = destImg;
            }
        }

        private void label21_Click(object sender, EventArgs e)
        {
            setAvin();
        }
        void setAvin()
        {
            if (flag == false)
            {
                source = 35;
                flag = true;
                label21.Image = sourceImg;
            }
            else
            {
                dest = 35;
                label21.Image = destImg;
            }
        }

        private void label40_Click(object sender, EventArgs e)
        {
            setTemple2();
        }
        void setTemple2()
        {
            if (flag == false)
            {
                source = 41;
                flag = true;
                label40.Image = sourceImg;
            }
            else
            {
                dest = 41;
                label40.Image = destImg;
            }
        }

        private void label38_Click(object sender, EventArgs e)
        {
            setLectureHall();

        }
        void setLectureHall()
        {
            if (flag == false)
            {
                source = 46;
                flag = true;
                label38.Image = sourceImg;
            }
            else
            {
                dest = 46;
                label38.Image = destImg;
            }
        }

        private void label20_Click(object sender, EventArgs e)
        {
            setAnnex();
        }
        void setAnnex()
        {
            if (flag == false)
            {
                source = 42;
                flag = true;
                label20.Image = sourceImg;
            }
            else
            {
                dest = 42;
                label20.Image = destImg;
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {
            setOctagon();
        }
        void setOctagon()
        {
            if (flag == false)
            {
                source = 47;
                flag = true;
                label19.Image = sourceImg;
            }
            else
            {
                dest = 47;
                label19.Image = destImg;
            }
        }

        private void label33_Click(object sender, EventArgs e)
        {
            setProductionEngineering();
        }
        void setProductionEngineering()
        {
            if (flag == false)
            {
                source = 43;
                flag = true;
                label33.Image = sourceImg;
            }
            else
            {
                dest = 43;
                label33.Image = destImg;
            }
        }

        private void label34_Click(object sender, EventArgs e)
        {
            setManagementStudies();
        }
        void setManagementStudies()
        {
            if (flag == false)
            {
                source = 48;
                flag = true;
                label34.Image = sourceImg;
            }
            else
            {
                dest = 48;
                label34.Image = destImg;
            }
        }

        private void label37_Click(object sender, EventArgs e)
        {
            setBuhari();
        }
        void setBuhari()
        {
            if (flag == false)
            {
                source = 44;
                flag = true;
                label37.Image = sourceImg;
            }
            else
            {
                dest = 44;
                label37.Image = destImg;
            }
        }

        private void label39_Click(object sender, EventArgs e)
        {
            setPostOffice();
        }
        void setPostOffice()
        {
            if (flag == false)
            {
                source = 49;
                flag = true;
                label39.Image = sourceImg;
            }
            else
            {
                dest = 49;
                label39.Image = destImg;
            }
        }

        private void label59_Click(object sender, EventArgs e)
        {
            setArchitecture();
        }
        void setArchitecture()
        {
            if (flag == false)
            {
                source = 74;
                flag = true;
                label59.Image = sourceImg;
            }
            else
            {
                dest = 74;
                label59.Image = destImg;
            }
        }

        private void label32_Click(object sender, EventArgs e)
        {
            setCEESAT();
        }
        void setCEESAT()
        {
            if (flag == false)
            {
                source = 52;
                flag = true;
                label32.Image = sourceImg;
            }
            else
            {
                dest = 52;
                label32.Image = destImg;
            }
        }

        private void label60_Click(object sender, EventArgs e)
        {
            setPGLectureHall();
        }
        void setPGLectureHall()
        {
            if (flag == false)
            {
                source = 75;
                flag = true;
                label60.Image = sourceImg;
            }
            else
            {
                dest = 75;
                label60.Image = destImg;
            }
        }

        private void label31_Click(object sender, EventArgs e)
        {
            setPowerHouse();
        }
        void setPowerHouse()
        {
            if (flag == false)
            {
                source = 51;
                flag = true;
                label31.Image = sourceImg;
            }
            else
            {
                dest = 51;
                label31.Image = destImg;
            }
        }

        private void label43_Click(object sender, EventArgs e)
        {
            setLysium();

        }
        void setLysium()
        {
            if (flag == false)
            {
                source = 54;
                flag = true;
                label43.Image = sourceImg;
            }
            else
            {
                dest = 54;
                label43.Image = destImg;
            }
        }

        private void label44_Click(object sender, EventArgs e)
        {
            setCA_CSE();
        }
        void setCA_CSE()
        {
            if (flag == false)
            {
                source = 56;
                flag = true;
                label44.Image = sourceImg;
            }
            else
            {
                dest = 56;
                label44.Image = destImg;
            }
        }

        private void label47_Click(object sender, EventArgs e)
        {
            setChemicalEngineering();

        }
        void setChemicalEngineering()
        {
            if (flag == false)
            {
                source = 61;
                flag = true;
                label47.Image = sourceImg;
            }
            else
            {
                dest = 61;
                label47.Image = destImg;
            }
        }

        private void label48_Click(object sender, EventArgs e)
        {
            setBarn();

        }
        void setBarn()
        {
            if (flag == false)
            {
                source = 59;
                flag = true;
                label48.Image = sourceImg;
            }
            else
            {
                dest = 59;
                label48.Image = destImg;
            }
        }

        private void label49_Click(object sender, EventArgs e)
        {
            setAdminBlock();
        }
        void setAdminBlock()
        {

            if (flag == false)
            {
                source = 79;
                flag = true;
                label49.Image = sourceImg;
            }
            else
            {
                dest = 79;
                label49.Image = destImg;
            }
        }

        private void label52_Click(object sender, EventArgs e)
        {
            setSBI();
        }
        void setSBI()
        {
            if (flag == false)
            {
                source = 58;
                flag = true;
                label52.Image = sourceImg;
            }
            else
            {
                dest = 58;
                label52.Image = destImg;
            }
        }

        void setSnacky()
        {
            if (flag == false)
            {
                source = 58;
                flag = true;
                label45.Image = sourceImg;
            }
            else
            {
                dest = 58;
                label45.Image = destImg;
            }
        }

        private void label45_Click(object sender, EventArgs e)
        {
            setSnacky();
        }

        private void label41_Click(object sender, EventArgs e)
        {
            setEMC();
        }
        void setEMC()
        {
            if (flag == false)
            {
                source = 57;
                flag = true;
                label41.Image = sourceImg;
            }
            else
            {
                dest = 57;
                label41.Image = destImg;
            }
        }

        private void label46_Click(object sender, EventArgs e)
        {
            setPhysicsDept();

        }

        void setPhysicsDept()
        {
            if (flag == false)
            {
                source = 60;
                flag = true;
                label46.Image = sourceImg;
            }
            else
            {
                dest = 60;
                label46.Image = destImg;
            }
        }

        void setEEEAudi()
        {
            if (flag == false)
            {
                source = 60;
                flag = true;
                label51.Image = sourceImg;
            }
            else
            {
                dest = 60;
                label51.Image = destImg;
            }
        }

        private void label51_Click(object sender, EventArgs e)
        {
            setEEEAudi();
        }

        private void label50_Click(object sender, EventArgs e)
        {
            setTPMechMain();
        }
        void setTPMechMain()
        {
            if (flag == false)
            {
                source = 68;
                flag = true;
                label50.Image = sourceImg;
            }
            else
            {
                dest = 68;
                label50.Image = destImg;
            }
        }

        private void label29_Click(object sender, EventArgs e)
        {
            setGarnetA();
        }
        void setGarnetA()
        {
            if (flag == false)
            {
                source = 28;
                flag = true;
                label29.Image = sourceImg;
            }
            else
            {
                dest = 28;
                label29.Image = destImg;
            }
        }

        private void label42_Click(object sender, EventArgs e)
        {
            setIIM();
        }
        void setIIM()
        {
            if (flag == false)
            {
                source = 71;
                flag = true;
                label42.Image = sourceImg;
            }
            else
            {
                dest = 71;
                label42.Image = destImg;
            }
        }

        private void label54_Click(object sender, EventArgs e)
        {
            setVolleyBallGround();

        }
        void setVolleyBallGround()
        {
           
            if (flag == false)
            {
                source = 68;
                flag = true;
                label54.Image = sourceImg;
            }
            else
            {
                dest = 68;
                label54.Image = destImg;
            }
        }
        private void label53_Click(object sender, EventArgs e)
        {

        }
        void setNSO()
        {
            if (flag == false)
            {
                source = 82;
                flag = true;
                label53.Image = sourceImg;
            }
            else
            {
                dest = 82;
                label53.Image = destImg;
            }
        }

        private void label55_Click(object sender, EventArgs e)
        {
            setFootballGround();


        }
        void setFootballGround()
        {
            if (flag == false)
            {
                source = 97;
                flag = true;
                label55.Image = sourceImg;
            }
            else
            {
                dest = 97;
                label55.Image = destImg;
            }
        }

        private void label61_Click(object sender, EventArgs e)
        {
            setSAC();
        }
        void setSAC()
        {
            if (flag == false)
            {
                source = 84;
                flag = true;
                label61.Image = sourceImg;
            }
            else
            {
                dest = 84;
                label61.Image = destImg;
            }
        }

        private void label56_Click(object sender, EventArgs e)
        {
            setBasketball();
        }
        void setBasketball()
        {
            if (flag == false)
            {
                source = 97;
                flag = true;
                label56.Image = sourceImg;
            }
            else
            {
                dest = 97;
                label56.Image = destImg;
            }
        }

        private void label58_Click(object sender, EventArgs e)
        {
            setOpal();
        }
        void setOpal()
        {
            if (flag == false)
            {
                source = 88;
                flag = true;
                label58.Image = sourceImg;
            }
            else
            {
                dest = 88;
                label58.Image = destImg;
            }
        }

        private void label67_Click(object sender, EventArgs e)
        {
            setLibrary();
        }
        void setLibrary()
        {
            if (flag == false)
            {
                source = 72;
                flag = true;
                label67.Image = sourceImg;
            }
            else
            {
                dest = 72;
                label67.Image = destImg;
            }

        }

        private void label62_Click(object sender, EventArgs e)
        {
            setFirstStreet();
        }
        void setFirstStreet()
        {
            if (flag == false)
            {
                source = 89;
                flag = true;
                label62.Image = sourceImg;
            }
            else
            {
                dest = 89;
                label62.Image = destImg;
            }

        }

        private void label63_Click(object sender, EventArgs e)
        {
            setSecondStreet();
        }
        void setSecondStreet()
        {
            if (flag == false)
            {
                source = 91;
                flag = true;
                label63.Image = sourceImg;
            }
            else
            {
                dest = 91;
                label63.Image = destImg;
            }

        }

        private void label64_Click(object sender, EventArgs e)
        {
            setThirdStreet();
        }
        void setThirdStreet()
        {
            if (flag == false)
            {
                source = 93;
                flag = true;
                label64.Image = sourceImg;
            }
            else
            {
                dest = 89;
                label64.Image = destImg;
            }

        }

        private void label65_Click(object sender, EventArgs e)
        {
            setFourthStreet();
        }
        void setFourthStreet()
        {
            if (flag == false)
            {
                source = 95;
                flag = true;
                label65.Image = sourceImg;
            }
            else
            {
                dest = 95;
                label65.Image = destImg;
            }

        }

        private void label66_Click(object sender, EventArgs e)
        {
            setStaffQtr();
        }
        void setStaffQtr()
        {
            if (flag == false)
            {
                source = 98;
                flag = true;
                label66.Image = sourceImg;
            }
            else
            {
                dest = 98;
                label66.Image = destImg;
            }

        }

        private void label68_Click(object sender, EventArgs e)
        {
            setNITTMainGate();
        }
        void setNITTMainGate()
        {
            if (flag == false)
            {
                source = 81;
                flag = true;
                label68.Image = sourceImg;
            }
            else
            {
                dest = 81;
                label68.Image = destImg;
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            source = comboBox1.SelectedIndex;
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dest = comboBox2.SelectedIndex;
        }

        private void label70_Click(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {
            
            if (refreshflag == false&&source>=0)
            {
                showAllPath();

                button20.Text = "ReFresh";
                refreshflag = true;
                scrollFlag = 2;
            }
            else
            {
                button20.Text = "ShowAllPath";
                refreshflag = false;
                comboBox3.Items.Clear();
                refreshPanels();
                scrollFlag = 0;
            }
            
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            sourceString = comboBox3.SelectedItem.ToString();
            
        }

        private void label72_Click(object sender, EventArgs e)
        {
            if (flag == false)
            {
                source = 14;
                flag = true;
                label72.Image = sourceImg;
            }
            else
            {
                dest = 14;
                label72.Image = destImg;
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

      
        
    }
}

