var roleId = "";
var selectedUsers = [];

$('.panel-default').mouseenter(function (e) {
    var recordsFound = $(this).find('.no-records-found').text();
    if (recordsFound) {
        var dataUrl = $(this).find('#TableData').val();
        $(this).find('table').bootstrapTable('refresh', {
            url: dataUrl
        });
    }
});

$('button[name="UserSelectBtns"]').click(function (e) {
    var current_target = $(e.currentTarget);
    roleId = current_target.val();
    var selectedUsers = [];
    var recordsFound = $('#userSelectModal').find('.no-records-found').text();
    if (recordsFound) {
        var dataUrl = $('#userSelectModal').find('#TableData').val();
        $('#userSelectModal').find('table').bootstrapTable('refresh', {
            url: dataUrl
        });
    }
    $("#userSelectModal").modal('show');
});

function userResposeHandler(res) {
    var selected_ids = $.map(selectedUsers, function (user, index) {
        return user.id;
    });
    $.each(res.rows, function (index, element) {
        if (selected_ids.indexOf(element.id) !== -1) {
            res.rows[index].check = true;
        }
    });
    return res;
}
function userQueryFormatter(params) {
    if (roleId !== "") {
        params.roleId = roleId;
    }
    return params;
}

$("#userSelectModal button:has(.glyphicon-ok)").click(function (e) {
    if (selectedUsers.length > 0) {
        var user_ids = $.map(selectedUsers, function (user, index) {
            return user.id;
        });
        var data = { roleId: roleId, users_ids: user_ids };
        $.post("/Role/UserAdd", data, function (res) {
            if (res === true) {
                selectedUsers = [];
                $('#roleUser_data_table').bootstrapTable('refresh');
                $("#userSelectModal").modal('hide');
            }
        });
    }
});

$('#usersList_data_table').on('check.bs.table uncheck.bs.table ' +
    'check-all.bs.table uncheck-all.bs.table', function (e, rows) {
        if ($.isArray(rows)) {
            if (e.type === "uncheck-all") {
                var selected_ids = $.map(rows, function (user, index) {
                    return user.id;
                });
                selectedUsers = $.map(selectedUsers, function (user, index) {
                    if ($.inArray(user.id, selected_ids) === -1) {
                        return user;
                    }
                });
            }
            else {
                $.each(rows, function (index, user) {
                    selectedUsers.push(user);
                });
            }
        }
        else {
            var deleted = false;
            selectedUsers = $.map(selectedUsers, function (user, index) {
                if (user.id !== rows.id) {
                    return user;
                }
                else {
                    deleted = true;
                }
            });
            if (!deleted) {
                selectedUsers.push(rows);
            }
        }
    });

function ChangeCheckPermission(obj, roleId) {
    $('.SelectedPermissions').attr("disabled", true);
    $('.objectEndpoint').attr("disabled", true);
    var data = { roleId: roleId, objectEndpointPermissionId: obj.value };
    if ($(obj).prop('checked')) {
        $.post('/Role/AddRolePermission', data, function (res) {
            if (res) {
                console.log('Сохранено, ' + roleId + ", " + obj.value);
            }
        });
    } else {
        $.post('/Role/DeleteRolePermission', data, function (res) {
            if (res) {
                console.log('Удалено ' + roleId + ", " + obj.value);
            }
        });
    }
    $('.SelectedPermissions').attr("disabled", false);
    $('.objectEndpoint').attr("disabled", false);
    CheckAllCheckBoxPermission(obj);
}

function ChangeCheckLocalPartition(obj, roleId) {
    $('.SelectedLocalPartition').attr("disabled", true);
    var data = { roleId: roleId, localPartitionId: obj.value };
    if ($(obj).prop('checked')) {
        $.post('/Role/AddLocalPartition', data, function (res) {
            if (res) {
                console.log('Сохранено, ' + roleId + ", " + obj.value);
            }
            $('.SelectedLocalPartition').attr("disabled", false);
        });
    } else {
        $.post('/Role/DeleteLocalPartition', data, function (res) {
            if (res) {
                console.log('Удалено, ' + roleId + ", " + obj.value);
            }
            $('.SelectedLocalPartition').attr("disabled", false);
        });
    }
    CheckAllCheckLocalPartition(obj);
}

$('#role_local_partitions_data_table').on('expand-row.bs.table', function (e, index, row, detail) {
    console.log(row.controller);
    var rolesContent = '<b>Список пуст</b>';
    var roleId = $('#RoleId').val();
    var data = { roleId: roleId, _controller: row.controller };
    $.get('/Role/_MethodListPartial', data, function (res) {
        if (res) {
            rolesContent = res;
        }
        $(detail).html(rolesContent);
    });
});

$('#role_permission_data_table').on('expand-row.bs.table', function (e, index, row, detail) {
    var rolesContent = '<b>Список пуст</b>';
    var data = { objectId: row.id, roleId: row.roleId };
    $.post('/Role/_ObjectEndpointListPartial', data, function (res) {
        if (res) {
            rolesContent = '<label class="row-md-12">Конечные точки:</label>';
            rolesContent += res;
        }
        $(detail).html(rolesContent);
        $(detail).find('input[class="SelectedPermissions"]').each(function () {
            CheckAllCheckBoxPermission(this);
        });
    });
});

$('#role_local_partitions_data_table').on('check.bs.table uncheck.bs.table', function (e, rows, $element) {
    var roleId = $('#RoleId').val();
    var data = { roleId: roleId, _controller: rows.controller };
    if (!$element.prop('checked')) {
        $.get('/Role/_DeleteControllerMethodsPartial', data, function (res) {
            if (res) {
                console.log('_DeleteControllerMethodsPartial');
            }
        });
        $($element).closest('tr').next().find('input[type="checkbox"][class="SelectedLocalPartition"]').prop('checked', '');
    } else {
        $.get('/Role/_AddControllerMethodsPartial', data, function (res) {
            if (res) {
                console.log('_AddControllerMethodsPartial');
            }
        });
        $($element).closest('tr').next().find('input[type="checkbox"][class="SelectedLocalPartition"]').prop('checked', 'checked');
    }
});

function CheckAllCheckBoxPermission(obj) {
    var all = 0;
    var check = 0;
    $(obj).closest('ul').find('input[class="SelectedPermissions"]').each(function () {
        all++;
        if ($(this).prop('checked')) {
            check++;
        }
    });
    if (all === check) {
        $(obj).closest('ul').closest('li').find('input[class="objectEndpoint"]').prop('checked', 'checked');
    } else {
        $(obj).closest('ul').closest('li').find('input[class="objectEndpoint"]').prop('checked', '');
    }
}

function CheckAllCheckLocalPartition(obj) {
    var all = 0;
    var check = 0;
    $(obj).closest('tr').find('input[class="SelectedLocalPartition"]').each(function () {
        all++;
        if ($(this).prop('checked')) {
            check++;
        }
    });
    if (all === check) {
        $(obj).closest('tr').prev().find('input[name="btSelectItem"]').prop('checked', 'checked');
    } else {
        $(obj).closest('tr').prev().find('input[name="btSelectItem"]').prop('checked', '');
    }
}

function SelectAllEndpoints(obj, roleId) {
    $('.SelectedPermissions').attr("disabled", true);
    $('.objectEndpoint').attr("disabled", true);
    $(obj).closest('li').find('input[class="SelectedPermissions"]').each(function () {
        var thisCheckBox = this;
        var data = { roleId: roleId, objectEndpointPermissionId: this.value };
        if ($(obj).prop('checked')) {
            $.post('/Role/AddRolePermission', data, function (res) {
                if (res) {
                    console.log('Сохранено, ' + roleId + ", " + this.value);
                    $(thisCheckBox).prop('checked', obj.checked);
                }
            });
        } else {
            $.post('/Role/DeleteRolePermission', data, function (res) {
                if (res) {
                    console.log('Удалено, ' + roleId + ", " + this.value);
                    $(thisCheckBox).prop('checked', obj.checked);
                }
            });
        }
    });
    $('.SelectedPermissions').attr("disabled", false);
    $('.objectEndpoint').attr("disabled", false);
}

window.roleEvents = {
    'click .details': function (e, value, row, index) {
        window.location.href = '/Role/Details?roleId=' + row.id;
    },
    'click .edit': function (e, value, row, index) {
        window.location.href = '/Role/Edit?roleId=' + row.id + '&roleName=' + row.name;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить запись?")) {
            var data = { Id: row.id, Name: row.name };

            $.post('/Role/Delete', data, function (res) {
                if (res) {
                    $('#role_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }
    }
};

window.userEvents = {
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить запись?")) {
            var roleId = $('#roleId').val();
            var data = { userId: row.id, roleId: roleId };

            $.post('/Role/DeleteUser', data, function (res) {
                if (res) {
                    $('#roleUser_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }
    }
};


function roleFormatter(value, row) {
    return [
        '<a class="details" href="javascript:void(0)" title="Детальная информация"> ',
        '<i class="glyphicon glyphicon-eye-open"></i>',
        '</a>  ',
        '<a class="edit" href="javascript:void(0)" title="Редактировать"> ',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function userFormatter(value, row) {
    return [
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function stateCheckedFormatter(value, row, index) {
    return row.state;
}

function userActivatedFormatter(value) {
    var checkbox = '<input type="checkbox" name="activated_checbox"' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
}
