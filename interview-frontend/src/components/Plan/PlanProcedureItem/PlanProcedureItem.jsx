import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";

import ReactSelect from "react-select";
import {
    addUserToProceudures,
    getUserProcedures
  } from "../../../api/api";


const PlanProcedureItem = ({ procedure, users }) => {
    const [selectedUsers, setSelectedUsers] = useState(null);

    let { id } = useParams();

    
    useEffect(() => {
        (async () => {
          var procedureUsers = await getUserProcedures(id);
          var slectedUserOptions = [];
          procedureUsers.map((u) => {
            if(u.planId == id && u.procedureId == procedure.procedureId)

                {
                    (slectedUserOptions.push({ label: u.user.name, value: u.userId }))
                }
        });
          setSelectedUsers(slectedUserOptions);
        })();
      }, [id]);

    const handleAssignUserToProcedure = async (e,v) => {
      
      if(v.action === "remove-value")
      {
        await addUserToProceudures(v.removedValue.value, id, procedure.procedureId);
        const removeOption = selectedUsers.filter((options) => options.value !== v.removedValue.value);

        setSelectedUsers(removeOption);
      }
      else{
        await addUserToProceudures(v.option.value, id, procedure.procedureId);
        setSelectedUsers((prevState) => {
          return [
            ...prevState,
            {
              label: v.option.label, value: v.option.value
            },
          ];
        });
      }
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div> 

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                key = {procedure.id}
                isClearable = {true}
                isMulti={true}
                options={users}
                value={selectedUsers}
            onChange={(e,v) => handleAssignUserToProcedure(e,v)}
            />
        </div>
    );
};

export default PlanProcedureItem;
