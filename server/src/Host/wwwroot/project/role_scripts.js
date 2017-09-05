
function GetChildrenPermission(e, roleId) {
    var children = $(e).closest('li');

    $(children).find('ul').remove();

    if ($(children).find('span').attr('class') === 'glyphicon glyphicon-minus') {
        $(children).find('span').attr('class', 'glyphicon glyphicon-plus');
    } else {
        var data;
        data = { tableId: e.id, type: $(children).attr('name'), roleId: roleId };
        $.get('/Role/GetChildrenPermission', data, function (res) {
            if (res) {
                $(children).find('span').attr('class', 'glyphicon glyphicon-minus');
                $(children).append(res);
            }
        });
    }
}

window.roleEvents = {
    //'click .permission': function (e, value, row, index) {
    //    document.location.replace('/Role/RolePermissionList?roleId=' + row.id);
    //},
    //'click .partitions': function (e, value, row, index) {
    //    document.location.replace('/Role/RoleLocalPartitionList?roleId=' + row.id);
    //},
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

function roleFormatter(value, row) {
    return [
        //'<a class="permission btn btn-default" href="javascript:void(0)" title="Разрешения">',
        //'<i class="glyphicon glyphicon-user"></i> Разрешения',
        //'</a>  ',
        //'<a class="partitions btn btn-default" href="javascript:void(0)" title="Доступные разделы">',
        //'<i class="glyphicon glyphicon-user"></i> Доступные разделы',
        //'</a>  ',
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