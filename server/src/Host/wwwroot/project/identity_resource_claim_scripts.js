
window.identityResourceClaimEvents = {
    'click .edit': function (e, value, row, index) {
        window.location.href = '/IdentityResourceClaim/Edit?identityResourceId=' + row.id;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить?")) {
            var data = { Id: row.id, Name: row.name };

            $.post('/IdentityResourceClaim/Delete', data, function (res) {
                if (res) {
                    $('#identityResourceClaim_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function identityResourceClaimFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактировать">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}