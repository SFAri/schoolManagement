import { ColDef } from "ag-grid-community";
import { AgGridReact } from "ag-grid-react";

// Cần thêm pageSize, PageSizeSelector nữa nma thêm sau đi
const CustomGridTable: React.FC<{rowData : any, colDefs: any, defaultColDef: ColDef, height: number, onCellValueChanged : any, gridRef: any}>= ({rowData, colDefs, defaultColDef, height =500, onCellValueChanged, gridRef}) => {
    return (
        <>
            <div style={{height: height}}>
                <AgGridReact
                    ref = {gridRef}
                    rowData={rowData}
                    columnDefs={colDefs}
                    defaultColDef={defaultColDef}
                    pagination={true}
                    paginationPageSize={10}
                    paginationPageSizeSelector={[10, 25, 50]}
                    onCellValueChanged={onCellValueChanged}
                />
            </div>
        </>
    )
}

export default CustomGridTable;