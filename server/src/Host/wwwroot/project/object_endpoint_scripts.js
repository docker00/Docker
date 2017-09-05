window.objectEndpointEvents = {
    'click .edit': function (e, value, row, index) {
        window.location.href = '/ObjectEndpoint/Edit?objectEndpointId=' + row.id;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить?")) {
            var data = { Id: row.id };

            $.post('/ObjectEndpoint/Delete', data, function (res) {
                if (res) {
                    $('#object_endpoint_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function objectEndpointFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактировать"> ',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}
