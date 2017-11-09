function ExportToExcel() {
    var htmltable = document.getElementById('example2');
    var html = htmltable.outerHTML;
    window.open('data:application/vnd.ms-excel,' + encodeURIComponent(html));
}