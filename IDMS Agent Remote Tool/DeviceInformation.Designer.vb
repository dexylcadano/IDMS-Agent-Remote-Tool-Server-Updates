<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmDeviceInformation
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Panel1 = New Panel()
        cmbOffices = New ComboBox()
        cmbUsers = New ComboBox()
        btnSave = New Button()
        Label3 = New Label()
        Label2 = New Label()
        txtPCName = New TextBox()
        Label1 = New Label()
        listUsers = New BindingSource(components)
        Panel1.SuspendLayout()
        CType(listUsers, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Panel1
        ' 
        Panel1.BorderStyle = BorderStyle.FixedSingle
        Panel1.Controls.Add(cmbOffices)
        Panel1.Controls.Add(cmbUsers)
        Panel1.Controls.Add(btnSave)
        Panel1.Controls.Add(Label3)
        Panel1.Controls.Add(Label2)
        Panel1.Controls.Add(txtPCName)
        Panel1.Controls.Add(Label1)
        Panel1.Location = New Point(12, 12)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(289, 238)
        Panel1.TabIndex = 0
        ' 
        ' cmbOffices
        ' 
        cmbOffices.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cmbOffices.AutoCompleteSource = AutoCompleteSource.ListItems
        cmbOffices.FormattingEnabled = True
        cmbOffices.Location = New Point(25, 160)
        cmbOffices.Name = "cmbOffices"
        cmbOffices.Size = New Size(248, 23)
        cmbOffices.TabIndex = 9
        ' 
        ' cmbUsers
        ' 
        cmbUsers.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cmbUsers.AutoCompleteSource = AutoCompleteSource.ListItems
        cmbUsers.FormattingEnabled = True
        cmbUsers.Location = New Point(25, 95)
        cmbUsers.Name = "cmbUsers"
        cmbUsers.Size = New Size(248, 23)
        cmbUsers.TabIndex = 8
        ' 
        ' btnSave
        ' 
        btnSave.Location = New Point(112, 196)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(75, 23)
        btnSave.TabIndex = 7
        btnSave.Text = "Save"
        btnSave.UseVisualStyleBackColor = True
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label3.Location = New Point(3, 139)
        Label3.Name = "Label3"
        Label3.Size = New Size(57, 15)
        Label3.TabIndex = 5
        Label3.Text = "Location:"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label2.Location = New Point(3, 74)
        Label2.Name = "Label2"
        Label2.Size = New Size(36, 15)
        Label2.TabIndex = 3
        Label2.Text = "User:"
        ' 
        ' txtPCName
        ' 
        txtPCName.Location = New Point(25, 30)
        txtPCName.Name = "txtPCName"
        txtPCName.ReadOnly = True
        txtPCName.Size = New Size(248, 23)
        txtPCName.TabIndex = 2
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label1.Location = New Point(3, 12)
        Label1.Name = "Label1"
        Label1.Size = New Size(60, 15)
        Label1.TabIndex = 1
        Label1.Text = "PC Name:"
        ' 
        ' frmDeviceInformation
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(313, 265)
        Controls.Add(Panel1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "frmDeviceInformation"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Device Information"
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(listUsers, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtPCName As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnSave As Button
    Friend WithEvents listUsers As BindingSource
    Friend WithEvents cmbUsers As ComboBox
    Friend WithEvents cmbOffices As ComboBox
End Class
