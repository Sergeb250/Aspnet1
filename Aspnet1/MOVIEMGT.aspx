<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MOVIEMGT.aspx.cs" Inherits="Aspnet1.MOVIEMGT" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>MOVIE MANAGEMENT</h3>
    
    <asp:Label ID="messagelbl" runat="server" CssClass="alert alert-success" Visible="false"></asp:Label><br />
    <asp:Label ID="errorlbl" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label><br />
     
    <asp:Label ID="movieIdlbl" runat="server" CssClass="form-control" Visible="false"></asp:Label><br />

   
    <div class="card mb-4">
        <div class="card-header">
            <h5>Import Movies from CSV File</h5>
        </div>
        <div class="card-body">
        
            <div class="border rounded p-4 text-center mb-3" 
                 style="border: 2px dashed #ccc; min-height: 150px;" 
                 id="dropArea">
                <i class="fas fa-cloud-upload-alt fa-3x text-muted mb-3"></i>
                <h5>Drag & Drop CSV file here</h5>
                <p class="text-muted">Supported format: CSV </p>
                <asp:FileUpload ID="fileUpload" runat="server" CssClass="d-none" AllowMultiple="false" Accept=".csv" />
                <asp:Button ID="browseBtn" runat="server" Text="Browse CSV Files" 
                           CssClass="btn btn-outline-primary mt-2" OnClientClick="triggerFileUpload(); return false;" />
            </div>
            

            <div class="row">
                <div class="col-md-8">
                    <asp:Label ID="fileNameLbl" runat="server" Text="No file selected" CssClass="text-muted"></asp:Label>
                    <div class="mt-2">
                        <small class="text-info">
                            <strong>CSV Format:</strong> Name,Director,Description,Rating<br />
                            
                        </small>
                    </div>
                </div>
                <div class="col-md-4 text-right">
                    <asp:Button ID="importBtn" runat="server" Text="Import Movies" 
                               CssClass="btn btn-primary" OnClick="importBtn_Click" Enabled="false" />
                </div>
            </div>
        </div>
    </div>

  
    <div class="card mb-4">
        <div class="card-header">
            <h5>Add/Edit Movie</h5>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <asp:Label ID="Label2" runat="server" Text="Movie Name" CssClass="font-weight-bold"></asp:Label>
                        <asp:TextBox ID="nameBox" runat="server" CssClass="form-control" placeholder="Enter movie name"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <asp:Label ID="Label3" runat="server" Text="Director" CssClass="font-weight-bold"></asp:Label>
                        <asp:TextBox ID="directorBox" runat="server" CssClass="form-control" placeholder="Enter director name"></asp:TextBox>
                    </div>
                </div>
            </div>
            
            <div class="form-group">
                <asp:Label ID="Label4" runat="server" Text="Description" CssClass="font-weight-bold"></asp:Label>
                <asp:TextBox ID="descriptionBox" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="3" placeholder="Enter movie description"></asp:TextBox>
            </div>
            
            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <asp:Label ID="Label5" runat="server" Text="Rating" CssClass="font-weight-bold"></asp:Label>
                        <asp:TextBox ID="ratingBox" runat="server" CssClass="form-control" 
                                   placeholder="Enter rating (0-10)" TextMode="Number" step="0.1" min="0" max="10"></asp:TextBox>
                    </div>
                </div>
            </div>
            
            <div class="form-group">
                <asp:Button ID="saveBtn" runat="server" CssClass="btn btn-success" Text="Save Movie" OnClick="saveBox_Click" /> 
                <asp:Button ID="updateBtn" runat="server" CssClass="btn btn-warning" Text="Update Movie" OnClick="updateBtn_Click" />
                <asp:Button ID="clearBtn" runat="server" CssClass="btn btn-secondary" Text="Clear Form" OnClick="clearBtn_Click" />
            </div>
        </div>
    </div>

  
    <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
            <h5 class="mb-0">Movie List</h5>
            <asp:Button ID="refreshBtn" runat="server" CssClass="btn btn-outline-info btn-sm" 
                       Text="Refresh" OnClick="refreshBtn_Click" />
        </div>
        <div class="card-body">
            <asp:GridView
                ID="movieGridView"
                runat="server"
                CssClass="table table-striped table-bordered table-hover"
                AutoGenerateColumns="False"
                OnRowCommand="movieGridView_RowCommand">
                <HeaderStyle CssClass="thead-dark" />
                <Columns>
                    <asp:BoundField DataField="movieId" HeaderText="Movie ID" ReadOnly="true" />
                    <asp:BoundField DataField="name" HeaderText="Name" />
                    <asp:BoundField DataField="director" HeaderText="Director" />
                    <asp:BoundField DataField="description" HeaderText="Description" />
                    <asp:BoundField DataField="rating" HeaderText="Rating" DataFormatString="{0:0.0}" />
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:Button ID="editBtn" runat="server" CssClass="btn btn-primary btn-sm" Text="Edit" 
                                      CommandArgument='<%# Eval("movieId") %>' CommandName="EditView" />
                            <asp:Button ID="deleteBtn" runat="server" CssClass="btn btn-danger btn-sm" Text="Delete" 
                                      CommandArgument='<%# Eval("movieId") %>' CommandName="DeleteMovie" 
                                      OnClientClick="return confirm('Are you sure you want to delete this movie?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="text-center text-muted p-4">
                        <i class="fas fa-film fa-3x mb-3"></i>
                        <p>No movies found. Add some movies or import from a CSV file.</p>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

    <script type="text/javascript">
        function triggerFileUpload() {
            document.getElementById('<%= fileUpload.ClientID %>').click();
        }

    
        document.addEventListener('DOMContentLoaded', function () {
            var dropArea = document.getElementById('dropArea');
            var fileUpload = document.getElementById('<%= fileUpload.ClientID %>');
            var fileNameLbl = document.getElementById('<%= fileNameLbl.ClientID %>');
            var importBtn = document.getElementById('<%= importBtn.ClientID %>');

          
            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
                dropArea.addEventListener(eventName, preventDefaults, false);
                document.body.addEventListener(eventName, preventDefaults, false);
            });

          
            ['dragenter', 'dragover'].forEach(eventName => {
                dropArea.addEventListener(eventName, highlight, false);
            });

            ['dragleave', 'drop'].forEach(eventName => {
                dropArea.addEventListener(eventName, unhighlight, false);
            });

          
            dropArea.addEventListener('drop', handleDrop, false);

           
            fileUpload.addEventListener('change', handleFileSelect, false);

            function preventDefaults(e) {
                e.preventDefault();
                e.stopPropagation();
            }

            function highlight() {
                dropArea.style.borderColor = '#007bff';
                dropArea.style.backgroundColor = '#f8f9fa';
            }

            function unhighlight() {
                dropArea.style.borderColor = '#ccc';
                dropArea.style.backgroundColor = '';
            }

            function handleDrop(e) {
                var dt = e.dataTransfer;
                var files = dt.files;
                handleFiles(files);
            }

            function handleFileSelect(e) {
                var files = e.target.files;
                handleFiles(files);
            }

            function handleFiles(files) {
                if (files.length > 0) {
                    var file = files[0];
                    var fileExtension = file.name.split('.').pop().toLowerCase();

                    if (fileExtension !== 'csv') {
                        alert('Please select a CSV file only.');
                        return;
                    }

                    fileNameLbl.innerText = file.name;
                    fileNameLbl.className = 'text-success font-weight-bold';
                    importBtn.disabled = false;

                   
                    fileUpload.files = files;
                }
            }
        });
    </script>

   
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
</asp:Content>