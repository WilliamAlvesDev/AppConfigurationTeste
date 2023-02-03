import React, { Fragment, FunctionComponent, useState, useEffect, useRef } from "react";
import { withStyles, Theme, createStyles, makeStyles } from "@material-ui/core/styles";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Paper from "@material-ui/core/Paper";
import InputBase from "@material-ui/core/InputBase";
import Divider from "@material-ui/core/Divider";
import IconButton from "@material-ui/core/IconButton";
import EditIcon from "@material-ui/icons/Edit";
import CheckIcon from "@material-ui/icons/Check";
import NotInterestedIcon from "@material-ui/icons/NotInterested";

import { FeatureSetting } from "../typings/models";
import { WarningIcon } from "../components/icons";

const StyledTableCell = withStyles((theme: Theme) =>
    createStyles({
        root: {
            padding: 8,
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
    root: {
        padding: "2px 4px",
        display: "flex",
        alignItems: "center",
        boxShadow: "none",
        border: "1px solid #e0e0e0",
    },
    input: {
        marginLeft: 8,
        flex: 1,
    },
    iconButton: {
        padding: 10,
    },
    divider: {
        height: 28,
        margin: 4,
    },
});

type FeatureSettingsTableProps = {
    data: FeatureSetting[];
    onChange: (key: string, value: string) => void;
}

const Input = ({ data, onChange, ...props }: any) => {
    const classes = useStyles();
    const inputRef = useRef<any>();

    const [disabled, setDisabled] = useState(true);

    useEffect(() => {
        if (inputRef.current)
            inputRef.current.value = data.value;
    }, [data, inputRef]);

    return (
        <Paper component="form" className={classes.root}>
            <InputBase
                {...props}
                disabled={disabled}
                className={classes.input}
                inputRef={inputRef} />
            <Divider className={classes.divider} orientation="vertical" />
            { disabled ? (
                <IconButton
                    className={classes.iconButton}
                    onClick={() => setDisabled(false)}
                >
                    <EditIcon color="primary" />
                </IconButton>) : (
                <Fragment>
                    <IconButton
                        color="primary"
                        className={classes.iconButton}
                        onClick={() => {
                            setDisabled(true);
                            onChange(data.id, inputRef.current.value);
                        }}
                    >
                        <CheckIcon />
                    </IconButton>
                    <IconButton
                        className={classes.iconButton}
                        onClick={() => {
                            inputRef.current.value = data.value;
                            setDisabled(true);
                        }}
                    >
                        <NotInterestedIcon color="error" />
                    </IconButton>
                </Fragment>)}
        </Paper>);
}

const featureSettingsTable: FunctionComponent<FeatureSettingsTableProps> = ({ data, onChange }: FeatureSettingsTableProps) => {
    const classes = useStyles();

    return (
        <TableContainer component={Paper} style={{ marginTop: 16, borderRadius: 0 }}>
            <Table className={classes.table}>
                <TableHead>
                    <TableRow>
                        <StyledTableCell style={{ width: "40%" }}>ID</StyledTableCell>
                        <StyledTableCell style={{ width: "40%" }}>DESCRIPTION</StyledTableCell>
                        <StyledTableCell style={{ width: "20%" }} align="center">VALUE</StyledTableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.length > 0 ? data.map((row: FeatureSetting) => (
                        <TableRow key={row.id}>
                            <StyledTableCell style={{ width: "40%" }}>
                                {row.id}
                            </StyledTableCell>
                            <StyledTableCell style={{ width: "40%" }}>
                                {row.description}
                            </StyledTableCell>
                            <StyledTableCell style={{ width: "20%" }} align="center">
                                <Input data={row} onChange={onChange} />
                            </StyledTableCell>
                        </TableRow>
                    )) : (
                        <TableRow>
                            <StyledTableCell colSpan={3}>
                                <div className={classes.iconContainer}><WarningIcon color="gray" />&nbsp;No feature settings found...</div>
                            </StyledTableCell>
                        </TableRow>)}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export { featureSettingsTable as FeatureSettingsTable };