#Requires -Modules AnyPackage.Msu

Describe Get-Package {
    Context 'with no parameters' {
        It 'should return results' {
            Get-Package |
            Should -Not -BeNullOrEmpty
        }
    }
}
