import React, { FunctionComponent } from "react";
import Switch from "@material-ui/core/Switch";
import { withStyles, Theme, createStyles, makeStyles } from "@material-ui/core/styles";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Paper from "@material-ui/core/Paper";

import { FeatureFlag } from "../typings/models";
import { WarningIcon } from "../components/icons";

const StyledTableCell = withStyles((theme: Theme) =>
    createStyles({
        head: {
            backgroundColor: theme.palette.common.black,
            color: theme.palette.common.white,
        },
        body: {
            fontSize: 14,
        },
    })
)(TableCell);

const SwitchStyled = withStyles((theme: Theme) =>
    createStyles({
        root: {
            width: 36,
            height: 20,
            padding: 0,
            display: "inline-flex",
        },
        switchBase: {
            padding: 2,
            color: theme.palette.grey[500],
            '&$checked': {
                transform: "translateX(16px)",
                color: theme.palette.common.white,
                '& + $track': {
                    opacity: 1,
                    backgroundColor: theme.palette.primary.main,
                    borderColor: theme.palette.primary.main,
                },
            },
        },
        thumb: {
            width: 16,
            height: 16,
            boxShadow: "none",
        },
        track: {
            border: `1px solid ${theme.palette.grey[500]}`,
            borderRadius: 20 / 2,
            opacity: 1,
            backgroundColor: theme.palette.common.white,
        },
        checked: {},
    })
)(Switch);

const useStyles = makeStyles({
    table: {
        minWidth: 700
    },
    iconContainer: {
        width: "100%",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
    },
});

type FeatureFlagsTableProps = {
    data: FeatureFlag[];
    onChange: (id: string) => void;
}

const featureFlagsTable: FunctionComponent<FeatureFlagsTableProps> = ({ data, onChange }: FeatureFlagsTableProps) => {
    const classes = useStyles();
    
    return (
        <TableContainer component={Paper} style={{ marginTop: 16, borderRadius: 0 }}>
            <Table className={classes.table}>
                <TableHead>
                    <TableRow>
                        <StyledTableCell style={{ width: "40%" }}>ID</StyledTableCell>
                        <StyledTableCell style={{ width: "50%" }}>DESCRIPTION</StyledTableCell>
                        <StyledTableCell style={{ width: "10%" }} align="center">ENABLED</StyledTableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.length > 0 ? data.map((row: FeatureFlag) => (
                        <TableRow key={row.value.id}>
                            <StyledTableCell style={{ width: "40%" }}>
                                {row.value.id}
                            </StyledTableCell>
                            <StyledTableCell style={{ width: "50%" }}>
                                {row.value.description}
                            </StyledTableCell>
                            <StyledTableCell style={{ width: "10%" }} align="center">
                                <SwitchStyled
                                    checked={row.value.enabled}
                                    onChange={() => {
                                        onChange(row.value.id);
                                    }}
                                />
                            </StyledTableCell>
                        </TableRow>
                    )) : (
                        <TableRow>
                            <StyledTableCell colSpan={3}>
                                <div className={classes.iconContainer}><WarningIcon color="gray" />&nbsp;No feature flags found...</div>
                            </StyledTableCell>
                        </TableRow>)}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export { featureFlagsTable as FeatureFlagsTable };