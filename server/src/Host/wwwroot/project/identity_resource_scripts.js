
window.identityResourceCustomEvents = {
    'click .details': function (e, value, row, index) {
        window.location.href = '/IdentityResource/Details?identityResourceId=' + row.id;
    },
    'click .edit': function (e, value, row, index) {
        window.location.href = '/IdentityResource/Edit?identityResourceId=' + row.id;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить запись?")) {
            var data = { identityResourceId: row.id };

            $.get('/IdentityResource/Delete', data, function (res) {
                if (res) {
                    $('#identityResource_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function identityResourceCustomFormatter(value, row) {
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

function identityResourceCustomCheckboxFormatter(value) {
    var checkbox = '<input type="checkbox" disabled="disabled" ' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
}