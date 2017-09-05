window.clientRelationEvents = {
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить запись?")) {
            
            var clientId = $('#ClientId').val();
            alert(clientId)
            var data = { fromClientId: row.clientId, toClientId: row.id };

            $.get('/Client/DeleteClientRelation', data, function (res) {
                if (res) {
                    $('#clientRelation_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }
    }
};

function clientRelationFormatter(value, row) {
    return [
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}