
window.claimEvents = {
    'click .edit': function (e, value, row, index) {
        window.location.href = '/Claim/Edit?claimId=' + row.id + '&claimType=' + row.type;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить запись?")) {
            var data = { Id: row.id, Type: row.type };

            $.post('/Claim/Delete', data, function (res) {
                if (res) {
                    $('#claim_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function claimFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактировать"> ',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}