﻿var dataTable

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name' },
            { data: 'email' },
            { data: 'phoneNumber' },
            { data: 'companyName' },
            { data: 'role' },
            {
                data: { id: 'id', lockoutEnd: 'lockoutEnd' },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                        <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; ">
                                    <i class="bi bi-lock-fill"></i> Locked
                                </a>
                                <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; ">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                         </div>
                        `
                    } else {
                        return `
                        <div class="text-center">
                              
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer;">
                                    <i class="bi bi-unlock-fill"></i> Unlocked
                                </a>
                                  <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; ">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                         </div>
                        `
                    }
                }
            },
        ]
    });
}


function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}

