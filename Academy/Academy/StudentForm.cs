using Academy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Academy
{
    public partial class StudentForm : Form
    {
        Student studentFounded = new Student();
        public StudentForm()
        {
            InitializeComponent();
            GenerateStudents();
            FillGroups();
        }



        private void GenerateStudents()
        {
            dgvStudents.Rows.Clear();

            using (AcademyEntities db = new AcademyEntities())
            {
                List<Student> students = db.Students.ToList();

                foreach (var student in students)
                {
                    dgvStudents.Rows.Add(student.Id,
                                         student.Name,
                                         student.Surname,
                                         student.Email,
                                         student.Group.Mentor.Name);
                }

            }
        }

        private void FillGroups()
        {
            using (AcademyEntities db = new AcademyEntities())
            {
                cmbGroups.DataSource = db.Groups.Select(g => g.Name).ToList();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnDelete.Enabled)
            {
                if (!UpdateStudent())
                {
                    MessageBox.Show("Tələbə yenilənmədi. Xəta baş verdi", "Xəta");
                    return;
                }
                
                MessageBox.Show("Tələbə yeniləndi");
            }
            else
            {
                if (!CreateStudent())
                {
                    MessageBox.Show("Tələbə yaradılmadı. Xəta baş verdi", "Xəta");
                    return;
                }

                MessageBox.Show("Tələbə yaradıldı");
            }
            GenerateStudents();
            ResetStudentForm();
        }

        private bool CreateStudent()
        {
            int affectedRows = 0;
            using (AcademyEntities db = new AcademyEntities())
            {

                int groupID = (db.Groups.Where(g => g.Name == cmbGroups.SelectedItem.ToString()).FirstOrDefault()).Id;
                Student student = new Student
                {
                    Name = txtName.Text,
                    Surname = txtSurname.Text,
                    Email = txtEmail.Text,
                    Birthday = dtpBirthday.Value,
                    Gender = rbMale.Checked = true ? true : false,
                    GroupID = groupID
                };

                db.Students.Add(student);

                affectedRows = db.SaveChanges();

            }
            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private bool UpdateStudent()
        {
            int affectedRows = 0;
            using (AcademyEntities db = new AcademyEntities())
            {
                int groupID = (db.Groups.Where(g => g.Name == cmbGroups.SelectedItem.ToString()).FirstOrDefault()).Id;

                Student newStu =  db.Students.Where(s => s.Id == studentFounded.Id).FirstOrDefault();

                newStu.Name = txtName.Text;
                newStu.Surname = txtSurname.Text;
                newStu.Email = txtEmail.Text;
                newStu.Birthday = dtpBirthday.Value;
                newStu.Gender = rbMale.Checked = true ? true : false;
                newStu.GroupID = groupID;

                affectedRows = db.SaveChanges();

            }
            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void dgvStudents_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int studentID = (int)dgvStudents.Rows[e.RowIndex].Cells[0].Value;

            using (AcademyEntities db = new AcademyEntities())
            {
                studentFounded = db.Students.Where(s => s.Id == studentID).FirstOrDefault();
                if (studentFounded != null)
                {
                    txtName.Text = studentFounded.Name;
                    txtSurname.Text = studentFounded.Surname;
                    txtEmail.Text = studentFounded.Email;
                    dtpBirthday.Value = (DateTime)studentFounded.Birthday;
                    if (studentFounded.Gender)
                    {
                        rbMale.Checked = true;
                    }
                    else
                    {
                        rbFemale.Checked = true;
                    }

                    cmbGroups.SelectedItem = studentFounded.Group.Name;

                }
            }

            btnSave.Text = "Update";
            btnDelete.Enabled = true;

        }
        
        private void ResetStudentForm()
        {
            txtName.Clear();
            txtSurname.Clear();
            txtEmail.Clear();
            dtpBirthday.Value = DateTime.Now.Date;
            rbFemale.Checked = true;
            cmbGroups.SelectedIndex = 0;

            btnSave.Text = "Save";
            btnDelete.Enabled = false;

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetStudentForm();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            var message = MessageBox.Show(studentFounded.Name + " " + studentFounded.Surname + " tələbəsini silmək istəyirsinizmi?", "Tələbə Silmə", MessageBoxButtons.OKCancel);

            if (DialogResult.OK != message)
            {
                return;
            }

            if (!DeleteStudent())
            {
                MessageBox.Show("Tələbə Silinmədi", "Diqqət");
                return;
            }

            MessageBox.Show("Tələbə Silindi");

            GenerateStudents();
            ResetStudentForm();
        }

        private bool DeleteStudent()
        {
            int affectedRows = 0;
            using (AcademyEntities db = new AcademyEntities())
            {

                Student newStu = db.Students.Where(s => s.Id == studentFounded.Id).FirstOrDefault();
                db.Students.Remove(newStu);
                affectedRows = db.SaveChanges();

            }
            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
