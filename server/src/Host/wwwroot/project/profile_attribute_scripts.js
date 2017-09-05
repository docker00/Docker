function show_load() {
    var modal = $('<div />');
    modal.addClass("modal");
    $('body').append(modal);
    var loading = $(".loading");
    loading.show();
    var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
    loading.css({ top: top, left: left });
}

$('form').submit(function () {
    var loading = '<div class="form-group"><div class="col-md-8">Обновление данных. Подождите...</div></div>';
    show_load();
    //$(this).html(loading);
    //console.log('+');
});

window.profileAttributeEvents = {
    'click .edit': function (e, value, row, index) {
        window.location.href = '/ProfileAttribute/Edit?profileAttributeId=' + row.id;
    },
    'click .remove': function (e, value, row, index) {
        if (confirm("Вы действительно хотите удалить?")) {
            var data = { Id: row.id, Name: row.name };

            $.post('/ProfileAttribute/Delete', data, function (res) {
                if (res) {
                    $('#profileAttribute_data_table').bootstrapTable('remove', {
                        field: 'id',
                        values: [row.id]
                    });
                }
            });
        }

    },
};

function profileAttributeFormatter(value, row) {
    return [
        '<a class="edit" href="javascript:void(0)" title="Редактировать">',
        '<i class="glyphicon glyphicon-pencil"></i>',
        '</a>  ',
        '<a class="remove" href="javascript:void(0)" title="Удалить">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function profileAttributeCheckboxFormatter(value) {
    var checkbox = '<input type="checkbox" disabled="disabled" ' + (value === true ? "checked" : "") + ' value="true"/>';
    return checkbox;
}