## Updating SimpleCalc Source Code

There is shared source code for all the SimpleCalc solutions that's in the `resources` folder. To update source code, uncomment the `Import` element in the `SimpleCalculator.csproj` for the solution you want to make changes to. When the solution is built the relevant shared resources will be copied into the solution.

  <!-- Disabling source code updating - uncomment and do build to update source code from shared resources -->
  <!-- <Import Project="..\..\resources\shared-resources.targets" Condition="Exists('..\..\resources\shared-resources.targets')" /> -->

Make changes to the shared source code in the `resources` folder and then build the solution. The shared source code will be copied into the solution.  Commit the changes to the solution and the shared source code.

Don't forget to update the other solutions that will be affected eg if you make changes to the XAML, make sure both the XAML solutions are updated.