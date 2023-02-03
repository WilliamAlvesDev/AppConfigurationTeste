import React from "react";
import { withStyles, Theme, createStyles, makeStyles } from "@material-ui/core/styles";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Paper from "@material-ui/core/Paper";
import Skeleton from "react-loading-skeleton";

const StyledTableCell = withStyles((theme: Theme) =>
    createStyles({
        root: {
            width: "calc(50% - 75px)"
        },
        head: {
            backgroundColor: theme.palette.common.black,
            color: theme.palette.common.white,
        },
        body: {
            fontSize: 14,
        },
    })
)(TableCell);

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

const skeletonFeatureFlag = () => {
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
                    <TableRow>
                        <StyledTableCell colSpan={3}>
                            <Skeleton />
                        </StyledTableCell>
                    </TableRow>
                    <TableRow>
                        <StyledTableCell colSpan={3}>
                            <Skeleton />
                        </StyledTableCell>
                    </TableRow>
                    <TableRow>
                        <StyledTableCell colSpan={3}>
                            <Skeleton />
                        </StyledTableCell>
                    </TableRow>
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export { skeletonFeatureFlag as SkeletonFeatureFlag };