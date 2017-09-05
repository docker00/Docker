

window.apiResourceCustomEvents = {
    'click .details': function (e, value, row, index) {
        window.location.href = '/ApiResource/Details?apiResourceId=' + row.id;
    },
    'click .edit': function (e, value, row, index) {
        window.location.href = '/ApiResource/Edit?apiResourceId=' + row.id;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить?")) {
            var data = { Id: row.id, Name: row.name };

            $.post('/ApiResource/Delete', data, function (res) {
                if (res) {
                    $('#apiResource_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function apiResourceCustomFormatter(value, row) {
    return [
        '<a class="details" href="javascript:void(0)" title="Детальная информация">',
        '<i class="glyphicon glyphicon-eye-open"></i>',
        '</a>  ',
        '<a class="edit" href="javascript:void(0)" title="Редактировать">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function apiResourceCustomCheckboxFormatter(value) {
    var checkbox = '<input type="checkbox" disabled="disabled" ' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
}