document.addEventListener('DOMContentLoaded', function () {
        
    document.getElementById('printBtn').addEventListener('click', function () {
        const tableHTML = document.getElementById('exportTableContainer').innerHTML;

        const printWindow = window.open('', '', 'width=900,height=700');

        printWindow.document.write('<html><head><title>Print Table</title>');
        printWindow.document.write('<style>');
        printWindow.document.write('table { width: 100%; border-collapse: collapse; font-family: Arial; font-size: 14px; }');
        printWindow.document.write('th, td { border: 1px solid #000; padding: 8px; text-align: left; }');
        printWindow.document.write('</style>');
        printWindow.document.write('</head><body>');
        printWindow.document.write(tableHTML);
        printWindow.document.write('</body></html>');

        printWindow.document.close();
        printWindow.focus();

        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 500);
    });

    const printReportBtn = document.getElementById('PrintReport');
    if (printReportBtn) {
        printReportBtn.addEventListener('click', function () {
            console.log("PrintReport button clicked");

            const printContentElement = document.getElementById('printArea');
            if (!printContentElement) {
                console.error("printArea not found");
                return;
            }

            const printContent = printContentElement.innerHTML;
            const printWindow = window.open('', '', 'width=800,height=600');
            printWindow.document.write('<html><head><title>Receiving Report</title>');
            printWindow.document.write('<style>body{font-family: Arial;} table{width:100%;border-collapse:collapse;} th, td{border:1px solid #ddd;padding:8px;} th{background:#f2f2f2;} .text-center{text-align:center;}</style>');
            printWindow.document.write('</head><body>');
                printWindow.document.write(printContent);
            printWindow.document.write('</body></html>');
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
        });
    } else {
        console.warn("PrintReport button not found");
    }
});
