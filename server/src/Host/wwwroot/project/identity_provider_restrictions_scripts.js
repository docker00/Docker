
window.identityProviderRestrictionEvents = {
    'click .edit': function (e, value, row, index) {
        window.location.href = '/IdentityProviderRestriction/Edit?identityProviderRestrictionId=' + row.id + '&identityProviderRestrictionName=' + row.name;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить запись?")) {
            var data = { Id: row.id, Name: row.name };

            $.post('/IdentityProviderRestriction/Delete', data, function (res) {
                if (res) {
                    $('#identityProvider_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};
//identityProviderRestrictionId, string identityProviderRestrictionName
function identityProviderRestrictionFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактировать"> ',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}