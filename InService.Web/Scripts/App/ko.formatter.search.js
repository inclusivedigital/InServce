function formatData(att) {
    if (att.loading) return att.text;
    var markup = "<div class='py-2'>";
    for (var i = 0; i < att.extra.length; i++) {
        markup += "<div class='small'>" + att.extra[i].Caption + ": <strong>" + att.extra[i].Value + "</strong></div>";
    }
    markup += "</div>";
    return markup;
}