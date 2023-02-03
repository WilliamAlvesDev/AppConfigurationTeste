import clsx from "clsx";
import React, { useState, Fragment } from "react";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import LockIcon from "@material-ui/icons/Lock";
import { green, red } from "@material-ui/core/colors";
import { makeStyles, withStyles } from "@material-ui/core/styles";
import { getToken, storeToken, removeToken } from "../services/authorization-service";

const useStyles = makeStyles({
    root: {
        background: "#f5f5f5",
        marginTop: -32,
        marginBottom: 16,
        marginLeft: -32,
        marginRight: -32,
        display: "flex",
        justifyContent: "flex-end",
        border: "solid 1px #f5f5f5",
        padding: "16px 32px",
        boxShadow: "0px 2px 1px -2px rgb(0 0 0/20%), 0px 1px 1px 0px rgb(0 0 0/14%), 0px 1px 3px 0px rgb(3 0 0/12%)"
    },
    authorizeButton: {
        padding: "8px 16px",
        borderRadius: 0,
        color: green[500],
        backgroundColor: "#f5f5f5",
        borderColor: green[500],
        borderWidth: 2
    },
    logoutButton: {
        padding: "8px 16px",
        borderRadius: 0,
        color: red[500],
        backgroundColor: "#f5f5f5",
        borderColor: red[500],
        borderWidth: 2
    },
    logout: {
        color: red[500],
    },
    icon: {
        color: green[500]
    },
    confirmButton: {
        color: "black",
        padding: "8px 16px",
        borderRadius: 0,
        minWidth: 148,
        backgroundColor: "#FFC709",
        borderColor: "#FFC709",
        borderWidth: 2,
        '&:hover': {
            backgroundColor: "#E9B60B",
        },
    },
    cancelButton: {
        color: "white",
        padding: "8px 16px",
        borderRadius: 0,
        minWidth: 148,
        backgroundColor: "#202020",
        borderColor: "white",
        borderWidth: 2,
        '&:hover': {
            backgroundColor: "#323232",
        },
    }
});


const DialogStyled = withStyles(() => ({
    paper: {
        minWidth: 720,
        borderRadius: 0,
        border: "solid 1px white"
    }
}))(Dialog);

const DialogTitleStyled = withStyles(() => ({
    root: {
        color: "white",
        backgroundColor: "#202020"
    }
}))(DialogTitle);

const DialogContentStyled = withStyles(() => ({
    root: {
        margin: "16px 0",
        padding: 16
    }
}))(DialogContent);

const DialogActionsStyled = withStyles(() => ({
    root: {
        padding: 16,
        backgroundColor: "#202020"
    }
}))(DialogActions);

const TextFieldStyled = withStyles(() => ({
    root: {
        '& .MuiInputBase-root': {
            borderRadius: 0
        },
        '& .Mui-focused': {
            '& .MuiOutlinedInput-notchedOutline': {
                borderColor: "black"
            }
        }
    }
}))(TextField);

const header = () => {
    const classes = useStyles();
    const [open, setOpen] = useState(false);
    const [canSubmit, setCanSubmit] = useState(false);
    const [token, setToken] = useState(getToken());
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    const handleOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setToken(e.target.value);
        setCanSubmit(e.target.value.length > 10);
    };

    const handleAuthorize = () => {
        setIsAuthenticated(true);
        storeToken(token);
        setOpen(false);
    };

    const handleLogout = () => {
        removeToken();
        setIsAuthenticated(false);
        setCanSubmit(false);
        setToken("");
        setOpen(false);
    };

    return (
        <Fragment>
            <div className={classes.root}>
                <Button
                    variant="outlined"
                    className={isAuthenticated ? clsx(classes.logoutButton, classes.logout) : classes.authorizeButton}
                    onClick={isAuthenticated ? handleLogout : handleOpen}
                    startIcon={<LockIcon className={isAuthenticated ? classes.logout : classes.icon} />}
                >
                    {token ? "Logout" : "Authorize"}
                </Button>
            </div>
            <DialogStyled open={open} onClose={handleClose} aria-labelledby="form-dialog-title">
                <DialogTitleStyled id="form-dialog-title">Authorization</DialogTitleStyled>
                <DialogContentStyled>
                    <DialogContentText>
                        Please insert a valid bearer token in the field below
                    </DialogContentText>
                    <TextFieldStyled
                        autoFocus
                        fullWidth
                        multiline
                        margin="dense"
                        id="authorization"
                        type="text"
                        variant="outlined"
                        rows={10}
                        value={token}
                        onChange={handleChange}
                    />
                </DialogContentStyled>
                <DialogActionsStyled>
                    <Button variant="outlined" onClick={handleClose} className={classes.cancelButton}>
                        Cancel
                    </Button>
                    <Button variant="outlined" disabled={!canSubmit} onClick={handleAuthorize} className={classes.confirmButton}>
                        Authorize
                    </Button>
                </DialogActionsStyled>
            </DialogStyled>
        </Fragment>
    );
}

export { header as Header };

