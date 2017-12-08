function checkTownsEquality() {
    let startTown = $('select.start-town').val();
    let endTown = $('select.end-town').val();

    if (startTown == endTown && startTown && endTown) {
        $('div.form-errors').show();
        event.preventDefault();
    } else {
        $('div.form-errors').hide();
    }
}