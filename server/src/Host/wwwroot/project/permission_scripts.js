
window.permissionEvents = {
    'click .edit': function (e, value, row, index) {
        window.location.href = '/permission/Edit?permissionId=' + row.id + '&permissionName=' + row.name;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить запись?")) {
            var data = { Id: row.id, Type: row.type };

            $.post('/permission/Delete', data, function (res) {
                if (res) {
                    $('#permission_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function permissionFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактировать"> ',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}